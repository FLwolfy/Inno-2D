using InnoBase;
using InnoInternal.Render.Impl;
using InnoInternal.Resource.Bridge;
using InnoInternal.Resource.Impl;
using Microsoft.Xna.Framework.Graphics;

namespace InnoInternal.Render.Bridge;

internal class MonoGameRenderTarget : IRenderTarget, IDisposable
{
    private readonly RenderTarget2D m_target;
    
    private static GraphicsDevice device => MonoGameRenderAPI.graphicsDevice;

    public MonoGameRenderTarget(int width, int height)
    {
        m_target = new RenderTarget2D(device, width, height, false,
            device.PresentationParameters.BackBufferFormat, DepthFormat.None);
    }

    public void Clear(Color color)
    {
        device.Clear(ToXnaColor(color));
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