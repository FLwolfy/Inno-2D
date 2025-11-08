using InnoInternal.Render.Impl;
using Veldrid;

using InnoFBDescription = InnoInternal.Render.Impl.FrameBufferDescription;
using VeldridFBDescription = Veldrid.FramebufferDescription;

namespace InnoInternal.Render.Bridge;

internal class VeldridFrameBuffer : IFrameBuffer
{
    private readonly GraphicsDevice m_graphicsDevice;
    
    private readonly ITexture? m_depthAttachment;
    private readonly ITexture[] m_colorAttachments;
    
    public int width => (int)inner.Width;
    public int height => (int)inner.Height;

    internal Framebuffer inner { get; }

    internal VeldridFrameBuffer(GraphicsDevice graphicsDevice, Framebuffer frameBuffer)
    {
        m_graphicsDevice = graphicsDevice;
        inner = frameBuffer;

        if (frameBuffer.DepthTarget != null)
        {
            m_depthAttachment = new VeldridTexture(graphicsDevice, frameBuffer.DepthTarget.Value.Target);
        }
        
        m_colorAttachments = frameBuffer.ColorTargets
            .Select(ct => new VeldridTexture(graphicsDevice, ct.Target))
            .ToArray<ITexture>();
    }
    
    public VeldridFrameBuffer(GraphicsDevice graphicsDevice, InnoFBDescription desc)
    {
        m_graphicsDevice = graphicsDevice;
        m_depthAttachment = desc.depthAttachment;
        m_colorAttachments = desc.colorAttachments;
        inner = m_graphicsDevice.ResourceFactory.CreateFramebuffer(ToVeldridFBDesc(desc));
    }
    
    public ITexture GetAttachment(int index)
    {
        return m_colorAttachments[index];
    }

    public ITexture? GetDepthAttachment()
    {
        return m_depthAttachment;
    }

    private VeldridFBDescription ToVeldridFBDesc(InnoFBDescription desc)
    {
        var depthTexture = (desc.depthAttachment as VeldridTexture)?.inner;
        var colorTextures = desc.colorAttachments
            .Select(ct => (ct as VeldridTexture)!.inner)
            .ToArray();
        
        return new VeldridFBDescription(depthTexture, colorTextures);
    }
    
    public void Dispose()
    {
        inner.Dispose();
    }
}