namespace InnoInternal.Render.Impl;

public interface IIndexBuffer : IDisposable
{
    void Update<T>(T[] data) where T : struct;
}