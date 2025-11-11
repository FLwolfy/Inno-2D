using InnoBase.Graphics;

namespace InnoInternal.Render.Impl;

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