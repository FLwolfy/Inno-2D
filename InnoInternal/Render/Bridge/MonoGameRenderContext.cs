using InnoBase;
using InnoInternal.Render.Impl;
using Microsoft.Xna.Framework.Graphics;

namespace InnoInternal.Render.Bridge;

internal class MonoGameRenderContext : IRenderContext
{
    private static readonly Color DEFAULT_SCREEN_COLOR = Color.LIGHTGRAY;
    private static GraphicsDevice device => MonoGameRenderAPI.graphicsDevice;

    public void BeginFrame()
    {
        device.SetRenderTarget(null);
        device.Clear(ToXnaColor(DEFAULT_SCREEN_COLOR));
    }

    public void EndFrame() { }
    
    private static Microsoft.Xna.Framework.Color ToXnaColor(Color c) =>
        new Microsoft.Xna.Framework.Color(c.r, c.g, c.b, c.a);
}
