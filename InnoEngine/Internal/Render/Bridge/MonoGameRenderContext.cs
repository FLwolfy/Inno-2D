using InnoEngine.Internal.Base;
using InnoEngine.Internal.Render.Impl;

using Microsoft.Xna.Framework.Graphics;

namespace InnoEngine.Internal.Render.Bridge;

internal class MonoGameRenderContext : IRenderContext
{
    private static readonly Color DEFAULT_SCREEN_COLOR = Color.LIGHTGRAY;

    private readonly GraphicsDevice m_device;

    public MonoGameRenderContext(GraphicsDevice device)
    {
        m_device = device;
    }

    public void BeginFrame()
    {
        m_device.SetRenderTarget(null);
        m_device.Clear(ToXnaColor(DEFAULT_SCREEN_COLOR));
    }

    public void EndFrame() { }
    
    private static Microsoft.Xna.Framework.Color ToXnaColor(Color c) =>
        new Microsoft.Xna.Framework.Color(c.r, c.g, c.b, c.a);
}
