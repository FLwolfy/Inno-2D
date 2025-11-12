
namespace InnoInternal.Render.Impl;

public interface IUniformBuffer : IDisposable
{
    String bufferName { get; }
    
    void Set<T>(ref T data) where T : unmanaged;
}