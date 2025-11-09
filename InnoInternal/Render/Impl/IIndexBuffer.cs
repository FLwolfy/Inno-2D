namespace InnoInternal.Render.Impl;

public interface IIndexBuffer : IDisposable
{
    void Set<T>(T[] data) where T : unmanaged;
}