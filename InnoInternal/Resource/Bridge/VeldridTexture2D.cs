using System.Runtime.InteropServices;

using InnoInternal.Resource.Impl;

using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Veldrid;

namespace InnoInternal.Resource.Bridge;

internal class VeldridTexture2D : ITexture2D
{
    private readonly Texture m_texture;
    private readonly TextureView m_view;

    internal VeldridTexture2D(Texture texture, TextureView view)
    {
        m_texture = texture;
        m_view = view;
    }

    private VeldridTexture2D(GraphicsDevice device, Texture texture)
    {
        m_texture = texture;
        m_view = device.ResourceFactory.CreateTextureView(m_texture);
    }

    public int width => (int)m_texture.Width;
    public int height => (int)m_texture.Height;

    public Texture rawTexture => m_texture;
    public TextureView textureView => m_view;

    public void Dispose()
    {
        m_view.Dispose();
        m_texture.Dispose();
    }

    public static VeldridTexture2D LoadFromFile(GraphicsDevice device, string? path)
    {
        if (path == null) return CreateWhitePixel(device);
        
        using var fs = File.OpenRead(path);
        return LoadFromStream(device, fs);
    }

    private static VeldridTexture2D LoadFromStream(GraphicsDevice device, Stream stream)
    {
        using var img = SixLabors.ImageSharp.Image.Load<Rgba32>(stream);
        img.Mutate(x => x.Flip(FlipMode.Vertical)); // 注意 Vulkan 的坐标系统

        TextureDescription desc = TextureDescription.Texture2D(
            (uint)img.Width, (uint)img.Height, mipLevels: 1, arrayLayers: 1,
            PixelFormat.R8_G8_B8_A8_UNorm,
            TextureUsage.Sampled);

        var texture = device.ResourceFactory.CreateTexture(desc);
        texture.Name = "Texture2D_Imported";

        Rgba32[] pixelData = new Rgba32[img.Width * img.Height];
        img.CopyPixelDataTo(MemoryMarshal.AsBytes(pixelData.AsSpan()));
        device.UpdateTexture(texture, MemoryMarshal.AsBytes(pixelData.AsSpan()), 0, 0, 0, (uint)img.Width, (uint)img.Height, 1, 0, 0);

        return new VeldridTexture2D(device, texture);
    }

    private static VeldridTexture2D CreateWhitePixel(GraphicsDevice device)
    {
        TextureDescription desc = TextureDescription.Texture2D(
            1, 1, 1, 1, PixelFormat.R8_G8_B8_A8_UNorm, TextureUsage.Sampled);

        var texture = device.ResourceFactory.CreateTexture(desc);
        var pixel = new byte[] { 255, 255, 255, 255 }; // RGBA white
        device.UpdateTexture(texture, pixel, 0, 0, 0, 1, 1, 1, 0, 0);

        return new VeldridTexture2D(device, texture);
    }
}
