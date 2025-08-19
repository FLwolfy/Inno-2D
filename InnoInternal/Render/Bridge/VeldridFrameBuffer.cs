using InnoBase;
using InnoInternal.Render.Impl;
using Veldrid;

namespace InnoInternal.Render.Bridge;

internal class VeldridFrameBuffer : IFrameBuffer
{
    private readonly GraphicsDevice m_graphicsDevice;
    
    public int width { get; }
    public int height { get; }
    
    internal Framebuffer inner { get; }

    public VeldridFrameBuffer(GraphicsDevice graphicsDevice, Framebuffer frameBuffer)
    {
        m_graphicsDevice = graphicsDevice;
        inner = frameBuffer;
    }
    
    public VeldridFrameBuffer(GraphicsDevice graphicsDevice, FrameBufferDescription desc)
    {
        m_graphicsDevice = graphicsDevice;
        
        // TODO
    }
    
    public void Clear(Color color)
    {
        throw new NotImplementedException();
    }
    
    public void Dispose()
    {
        // TODO release managed resources here
    }
}