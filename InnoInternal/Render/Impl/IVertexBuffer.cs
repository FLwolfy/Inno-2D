namespace InnoInternal.Render.Impl;

public interface IVertexBuffer : IDisposable
{
    void Update<T>(T[] data) where T : unmanaged;
}