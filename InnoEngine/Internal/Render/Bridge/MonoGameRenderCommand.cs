using InnoEngine.Internal.Base;
using InnoEngine.Internal.Render.Impl;

using Microsoft.Xna.Framework.Graphics;

namespace InnoEngine.Internal.Render.Bridge;

internal class MonoGameRenderCommand : IRenderCommand
{
    private readonly GraphicsDevice m_device;

    public MonoGameRenderCommand(GraphicsDevice device)
    {
        m_device = device ?? throw new ArgumentNullException(nameof(device));
    }

    public void Clear(Color color)
    {
        m_device.Clear(ToXnaColor(color));
    }

    public void SetViewport(Rect viewport)
    {
        m_device.Viewport = new Viewport(viewport.x, viewport.y, viewport.width, viewport.height);
    }

    public void SetRenderTarget(IRenderTarget? target)
    {
        m_device.SetRenderTarget((target as MonoGameRenderTarget)?.rawTarget);
    }
    
    public IRenderTarget CreateRenderTarget(int width, int height)
    {
        return new MonoGameRenderTarget(m_device, width, height);
    }
    
    private static Microsoft.Xna.Framework.Color ToXnaColor(Color c) =>
        new Microsoft.Xna.Framework.Color(c.r, c.g, c.b, c.a);
}