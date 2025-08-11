namespace InnoInternal.Render.Impl;

public interface ITexture : IDisposable
{
    int width { get; }
    int height { get; }
}