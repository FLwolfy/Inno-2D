using InnoBase;
using InnoInternal.Render.Impl;
using Microsoft.Xna.Framework.Graphics;

namespace InnoInternal.Render.Bridge;

internal class MonoGameRenderContext : IRenderContext
{
    private static readonly Color DEFAULT_SCREEN_COLOR = Color.LIGHTGRAY;
    private static GraphicsDevice device => MonoGameRenderAPI.graphicsDevice;

    public Matrix? cameraMatrix { get; set; }

    public Vector2 GetWindowSize()
    {
        var pp = device.PresentationParameters;
        return new Vector2(pp.BackBufferWidth, pp.BackBufferHeight);
    }

    public Vector2 GetRenderTargetSize()
    {
        RenderTargetBinding[] targets = device.GetRenderTargets();
        if (targets.Length > 0)
        {
            // Only allow one render target for now
            var rt = (RenderTarget2D)targets[0].RenderTarget;
            return new Vector2(rt.Width, rt.Height);
        }
        else
        {
            // Back buffer
            var vp = device.Viewport;
            return new Vector2(vp.Width, vp.Height);
        }
    }

    public void BeginFrame()
    {
        device.Clear(ToXnaColor(DEFAULT_SCREEN_COLOR));
    }

    public void EndFrame() { }
    
    private static Microsoft.Xna.Framework.Color ToXnaColor(Color c) =>
        new Microsoft.Xna.Framework.Color(c.r, c.g, c.b, c.a);
}
