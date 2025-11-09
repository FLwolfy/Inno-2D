namespace InnoInternal.Render.Impl;

public enum PixelFormat
{
    B8G8R8A8UNorm,
    D32FloatS8UInt
}

[Flags]
public enum TextureUsage : byte
{
    // NOTE: This is copied from Veldrid.TextureUsage.
    //       Values must match exactly.
    Sampled = 1 << 0,
    Storage = 1 << 1,
    RenderTarget = 1 << 2,
    DepthStencil = 1 << 3,
    Cubemap = 1 << 4,
    Staging = 1 << 5,
    GenerateMipmaps = 1 << 6,
}

public enum TextureDimension
{
    Texture1D,
    Texture2D
}

public struct TextureDescription
{
    public int width;
    public int height;
    public PixelFormat format;
    public TextureUsage usage;
    public TextureDimension dimension;
}

public interface ITexture : IDisposable
{
    int width { get; }
    int height { get; }
}