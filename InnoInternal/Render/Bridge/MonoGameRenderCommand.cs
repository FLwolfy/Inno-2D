using InnoBase;
using InnoInternal.Render.Impl;

using Microsoft.Xna.Framework.Graphics;

using Color = InnoBase.Color;

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
        RenderTarget2D? rawTarget = (target as MonoGameRenderTarget)?.rawTarget;
        device.SetRenderTarget(rawTarget);

        if (rawTarget != null)
        {
            device.ScissorRectangle = new Microsoft.Xna.Framework.Rectangle(0, 0, rawTarget.Width, rawTarget.Height);
        }
        else
        {
            var viewport = device.Viewport;
            device.ScissorRectangle = new Microsoft.Xna.Framework.Rectangle(0, 0, viewport.Width, viewport.Height);
        }
    }

    
    public IRenderTarget CreateRenderTarget(int width, int height)
    {
        return new MonoGameRenderTarget(width, height);
    }
    
    private static Microsoft.Xna.Framework.Color ToXnaColor(Color c) =>
        new Microsoft.Xna.Framework.Color(c.r, c.g, c.b, c.a);
}