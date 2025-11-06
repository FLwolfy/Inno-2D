using InnoInternal.Render.Impl;

using Veldrid;

namespace InnoInternal.Render.Bridge;

internal class VeldridUniformBuffer : IUniformBuffer
{
    private readonly GraphicsDevice m_graphicsDevice;
    internal DeviceBuffer inner { get; }
    
    public string bufferName { get; }

    public VeldridUniformBuffer(GraphicsDevice graphicsDevice, DeviceBuffer indexBuffer, String name)
    {
        m_graphicsDevice = graphicsDevice;
        inner = indexBuffer;
        bufferName = name;
    }
    
    public void Update<T>(ref T data) where T : unmanaged
    {
        m_graphicsDevice.UpdateBuffer(inner, 0, ref data);
    }

    public void Dispose()
    {
        inner.Dispose();
    }
    
}