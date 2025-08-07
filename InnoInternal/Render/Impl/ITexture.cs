namespace InnoInternal.Render.Impl;

public interface ITexture : IDisposable
{
    int Width { get; }
    int Height { get; }
}