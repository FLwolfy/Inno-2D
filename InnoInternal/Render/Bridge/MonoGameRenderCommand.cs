using InnoBase;
using InnoInternal.Render.Impl;
using Microsoft.Xna.Framework.Graphics;

namespace InnoInternal.Render.Bridge;

internal class MonoGameRenderCommand : IRenderCommand
{
    private static GraphicsDevice device => MonoGameRenderAPI.graphicsDevice;
    
    public void Clear(Color color)
    {
        device.Clear(ToXnaColor(color));
    }

    public void SetViewport(Rect viewport)
    {
        device.Viewport = new Viewport(viewport.x, viewport.y, viewport.width, viewport.height);
    }

    public void SetRenderTarget(IRenderTarget? target)
    {
        device.SetRenderTarget((target as MonoGameRenderTarget)?.rawTarget);
    }
    
    public IRenderTarget CreateRenderTarget(int width, int height)
    {
        return new MonoGameRenderTarget(width, height);
    }
    
    private static Microsoft.Xna.Framework.Color ToXnaColor(Color c) =>
        new Microsoft.Xna.Framework.Color(c.r, c.g, c.b, c.a);
}