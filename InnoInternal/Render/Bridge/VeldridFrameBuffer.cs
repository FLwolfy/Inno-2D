using InnoInternal.Render.Impl;
using Veldrid;

using InnoFBDescription = InnoInternal.Render.Impl.FrameBufferDescription;
using VeldridFBDescription = Veldrid.FramebufferDescription;

namespace InnoInternal.Render.Bridge;

internal class VeldridFrameBuffer : IFrameBuffer
{
    private readonly GraphicsDevice m_graphicsDevice;
    
    private InnoFBDescription m_frameBufferDescription;
    private ITexture? m_depthAttachment;
    private ITexture[] m_colorAttachments;
    
    public int width { get; private set; }
    public int height { get; private set; }

    internal Framebuffer inner { get; private set; }

    // TODO: After applying texture shaders, remove this totally
    [Obsolete]
    internal VeldridFrameBuffer(GraphicsDevice graphicsDevice, Framebuffer frameBuffer)
    {
        m_graphicsDevice = graphicsDevice;
        
        inner = frameBuffer;

        if (frameBuffer.DepthTarget != null)
        {
            m_depthAttachment = new VeldridTexture(frameBuffer.DepthTarget.Value.Target);
        }
        
        m_colorAttachments = frameBuffer.ColorTargets
            .Select(ct => new VeldridTexture(ct.Target))
            .ToArray<ITexture>();
    }
    
    public VeldridFrameBuffer(GraphicsDevice graphicsDevice, InnoFBDescription desc)
    {
        m_graphicsDevice = graphicsDevice;
        
        width = desc.width;
        height = desc.height;
        
        EnsureTextureSize(ref desc);

        var colorTextures = new List<ITexture>();
        foreach (var cad in desc.colorAttachmentDescriptions)
        {
            colorTextures.Add(VeldridTexture.Create(graphicsDevice, cad));
        }
        m_colorAttachments = colorTextures.ToArray();
        m_depthAttachment = desc.depthAttachmentDescription == null ? null : VeldridTexture.Create(graphicsDevice, desc.depthAttachmentDescription.Value);
        
        m_frameBufferDescription = desc;
        inner = m_graphicsDevice.ResourceFactory.CreateFramebuffer(ToVeldridFBDesc(desc));
    }

    private void RecreateInner()
    {
        // Dispose Textures
        Dispose();
        
        // Ensure Texture Size
        EnsureTextureSize(ref m_frameBufferDescription);
        
        // Recreate inner and Textures
        var colorTextures = new List<ITexture>();
        foreach (var cad in m_frameBufferDescription.colorAttachmentDescriptions)
        {
            colorTextures.Add(VeldridTexture.Create(m_graphicsDevice, cad));
        }
        m_colorAttachments = colorTextures.ToArray();
        m_depthAttachment = m_frameBufferDescription.depthAttachmentDescription == null ? null : VeldridTexture.Create(m_graphicsDevice, m_frameBufferDescription.depthAttachmentDescription.Value);
        
        inner = m_graphicsDevice.ResourceFactory.CreateFramebuffer(ToVeldridFBDesc(m_frameBufferDescription));
    }

    private void EnsureTextureSize(ref InnoFBDescription desc)
    {
        for (int i = 0; i < desc.colorAttachmentDescriptions.Length; i++)
        {
            desc.colorAttachmentDescriptions[i].width = desc.width;
            desc.colorAttachmentDescriptions[i].height = desc.height;
        }

        if (desc.depthAttachmentDescription.HasValue)
        {
            var depthAttachmentCopy = desc.depthAttachmentDescription.Value;
            depthAttachmentCopy.width = desc.width;
            depthAttachmentCopy.height = desc.height;
            desc.depthAttachmentDescription = depthAttachmentCopy;
        }
    }

    public void Resize(int newWidth, int newHeight)
    {
        width = newWidth;
        height = newHeight;
        
        RecreateInner();
    }
    
    public ITexture? GetColorAttachment(int index)
    {
        if (index < 0 || index >= m_colorAttachments.Length) return null;
        return m_colorAttachments[index];
    }

    public ITexture? GetDepthAttachment()
    {
        return m_depthAttachment;
    }

    private VeldridFBDescription ToVeldridFBDesc(InnoFBDescription desc)
    {
        var depthTexture = (m_depthAttachment as VeldridTexture)?.inner;
        var colorTextures = m_colorAttachments
            .Select(ct => (ct as VeldridTexture)!.inner)
            .ToArray();
        
        return new VeldridFBDescription(depthTexture, colorTextures);
    }
    
    public void Dispose()
    {
        m_depthAttachment?.Dispose();
        
        foreach (var colorAttachment in m_colorAttachments)
        {
            colorAttachment.Dispose();
        }
        
        inner.Dispose();
    }
}