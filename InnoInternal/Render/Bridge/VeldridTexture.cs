using InnoBase.Graphics;
using InnoInternal.Render.Impl;
using Veldrid;

using InnoTEXDescription = InnoInternal.Render.Impl.TextureDescription;
using VeldridTEXDescription = Veldrid.TextureDescription;
using InnoPixelFormat = InnoBase.Graphics.PixelFormat;
using VeldridPixelFormat = Veldrid.PixelFormat;
using InnoTextureUsage = InnoBase.Graphics.TextureUsage;
using VeldridTextureUsage = Veldrid.TextureUsage;

using VTexture = Veldrid.Texture;

namespace InnoInternal.Render.Bridge;

internal class VeldridTexture : ITexture
{
    public int width { get; }
    public int height { get; }
    
    internal readonly Texture inner;

    public VeldridTexture(Texture inner)
    {
        this.inner = inner;
        width = (int)inner.Width;
        height = (int)inner.Height;
    }

    public static VeldridTexture Create(GraphicsDevice graphicsDevice, InnoTEXDescription desc)
    {
        VTexture vTexture = graphicsDevice.ResourceFactory.CreateTexture(ToVeldridTEXDesc(desc));
        return new VeldridTexture(vTexture);
    }

    private static VeldridTEXDescription ToVeldridTEXDesc(InnoTEXDescription desc)
    {
        var width = (uint) desc.width;
        var height = (uint) desc.height;

        return new VeldridTEXDescription(
            width,
            height,
            1,
            1,
            1,
            ToVeldridPixelFormat(desc.format),
            ToVeldridTextureUsage(desc.usage),
            ToVeldridTextureType(desc.dimension),
            TextureSampleCount.Count1);
    }
    
    internal static VeldridPixelFormat ToVeldridPixelFormat(InnoPixelFormat format)
    {
        return format switch
        {
            InnoPixelFormat.B8_G8_R8_A8_UNorm => VeldridPixelFormat.B8_G8_R8_A8_UNorm,
            InnoPixelFormat.D32_Float_S8_UInt => VeldridPixelFormat.D32_Float_S8_UInt,
            _ => throw new NotSupportedException($"Unsupported pixel format: {format}")
        };
    }

    internal static VeldridTextureUsage ToVeldridTextureUsage(InnoTextureUsage usage)
    {
        return (VeldridTextureUsage)(byte)usage;
    }

    internal static TextureType ToVeldridTextureType(TextureDimension dim)
    {
        return dim switch
        {
            TextureDimension.Texture1D => TextureType.Texture1D,
            TextureDimension.Texture2D => TextureType.Texture2D,
            _ => throw new NotSupportedException($"Unsupported texture type: {dim}")
        };
    }
    
    public void Dispose()
    {
        inner.Dispose();
    }
}