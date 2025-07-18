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

    public void SetViewport(Rect? viewport)
    {
        if (viewport == null)
        {
            device.Viewport = new Viewport(0, 0, device.PresentationParameters.BackBufferWidth, device.PresentationParameters.BackBufferHeight);
        }
        else
        {
            var v = viewport.Value;
            device.Viewport = new Viewport(v.x, v.y, v.width, v.height);
        }
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