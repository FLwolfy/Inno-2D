using InnoBase;
using InnoInternal.Render.Impl;
using Veldrid;

namespace InnoInternal.Render.Bridge;

internal class VeldridFrameBuffer : IFrameBuffer
{
    private readonly GraphicsDevice m_graphicsDevice;
    
    public int width => (int)inner.Width;
    public int height => (int)inner.Height;

    internal Framebuffer inner { get; }

    internal VeldridFrameBuffer(GraphicsDevice graphicsDevice, Framebuffer frameBuffer)
    {
        m_graphicsDevice = graphicsDevice;
        inner = frameBuffer;
    }
    
    public VeldridFrameBuffer(GraphicsDevice graphicsDevice, FrameBufferDescription desc)
    {
        m_graphicsDevice = graphicsDevice;
        
        // TODO
    }
    
    public ITexture GetAttachment(int index)
    {
        throw new NotImplementedException();
    }

    public ITexture? GetDepthAttachment()
    {
        throw new NotImplementedException();
    }
    
    public void Clear(Color color)
    {
        throw new NotImplementedException();
    }

    public void Resize(int width, int height)
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        // TODO release managed resources here
    }
}