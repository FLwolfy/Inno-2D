using InnoEngine.Internal.Base;
using InnoEngine.Internal.Render.Impl;
using InnoEngine.Internal.Resource.Bridge;
using InnoEngine.Internal.Resource.Impl;
using Microsoft.Xna.Framework.Graphics;

namespace InnoEngine.Internal.Render.Bridge;

internal class MonoGameRenderTarget : IRenderTarget, IDisposable
{
    private readonly RenderTarget2D m_target;
    private readonly GraphicsDevice m_device;

    public MonoGameRenderTarget(GraphicsDevice device, int width, int height)
    {
        m_device = device;
        m_target = new RenderTarget2D(device, width, height, false,
            device.PresentationParameters.BackBufferFormat, DepthFormat.None);
    }

    public void Bind()
    {
        m_device.SetRenderTarget(m_target);
    }
    
    public void Unbind()
    {
        m_device.SetRenderTarget(null);
    }

    public void Clear(Color color)
    {
        m_device.Clear(ToXnaColor(color));
    }

    public ITexture2D GetColorTexture()
    {
        return new MonoGameTexture2D(m_target);
    }

    public RenderTarget2D rawTarget => m_target;

    private Microsoft.Xna.Framework.Color ToXnaColor(Color c) => new(c.r, c.g, c.b, c.a);

    public void Dispose()
    {
        m_target?.Dispose();
    }
}