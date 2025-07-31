using InnoInternal.Render.Impl;
using InnoInternal.Resource.Bridge;
using InnoInternal.Resource.Impl;

using Veldrid;

namespace InnoInternal.Render.Bridge;

internal class VeldridRenderTarget : IRenderTarget
{
    private readonly Framebuffer m_framebuffer;
    private readonly Texture m_colorTexture;
    private readonly TextureView m_colorView;
    
    public uint width => m_framebuffer.Width;
    public uint height => m_framebuffer.Height;
    public Framebuffer rawFramebuffer => m_framebuffer;

    public VeldridRenderTarget(GraphicsDevice device, uint width, uint height)
    {
        // Create color texture
        TextureDescription colorDesc = TextureDescription.Texture2D(
            (uint)width, (uint)height, mipLevels: 1, arrayLayers: 1,
            PixelFormat.R8_G8_B8_A8_UNorm,
            TextureUsage.RenderTarget | TextureUsage.Sampled);

        m_colorTexture = device.ResourceFactory.CreateTexture(ref colorDesc);
        m_colorView = device.ResourceFactory.CreateTextureView(m_colorTexture);

        // Create framebuffer
        FramebufferDescription fbDesc = new FramebufferDescription(null, m_colorTexture);
        m_framebuffer = device.ResourceFactory.CreateFramebuffer(ref fbDesc);
    }

    public ITexture2D GetColorTexture()
    {
        return new VeldridTexture2D(m_colorTexture, m_colorView);
    }
    
    public void Dispose()
    {
        m_framebuffer.Dispose();
        m_colorView.Dispose();
        m_colorTexture.Dispose();
    }
}