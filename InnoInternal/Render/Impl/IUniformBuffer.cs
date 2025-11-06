
namespace InnoInternal.Render.Impl;

public interface IUniformBuffer : IDisposable
{
    String bufferName { get; }
    
    void Update<T>(ref T data) where T : unmanaged;
}