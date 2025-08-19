namespace InnoInternal.Render.Impl;

public struct TextureDescription
{
    public int width;
    public int height;
}

public interface ITexture : IDisposable
{
    int width { get; }
    int height { get; }
}