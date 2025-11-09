namespace InnoInternal.Render.Impl;

public interface IVertexBuffer : IDisposable
{
    void Set<T>(T[] data) where T : unmanaged;
}