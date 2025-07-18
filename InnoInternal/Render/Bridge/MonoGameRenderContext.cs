using InnoBase;
using InnoInternal.Render.Impl;

namespace InnoInternal.Render.Bridge;

internal class MonoGameRenderContext : IRenderContext
{
    private static readonly Color DEFAULT_SCREEN_COLOR = Color.LIGHTGRAY;
    private static Microsoft.Xna.Framework.Graphics.GraphicsDevice device => MonoGameRenderAPI.graphicsDevice;

    private Matrix m_cachedViewMatrix = Matrix.identity;
    private Matrix m_cachedProjectionMatrix = Matrix.identity;
    private Matrix m_cachedWorldToScreenMatrix = Matrix.identity;
    private Vector2 m_cachedRenderTargetSize = Vector2.ZERO;
    
    private bool m_matrixDirty = true;

    public Matrix viewMatrix
    {
        get => m_cachedViewMatrix;
        set
        {
            m_cachedViewMatrix = value;
            m_matrixDirty = true;
        }
    }

    public Matrix projectionMatrix
    {
        get => m_cachedProjectionMatrix;
        set
        {
            m_cachedProjectionMatrix = value;
            m_matrixDirty = true;
        }
    }

    public Matrix worldToScreenMatrix
    {
        get
        {
            Vector2 currentSize = GetRenderTargetSize();
            if (m_matrixDirty || currentSize != m_cachedRenderTargetSize)
            {
                Matrix viewProjectionMatrix = m_cachedViewMatrix * m_cachedProjectionMatrix;
                Matrix scale = Matrix.CreateScale(currentSize.x * 0.5f, currentSize.y * 0.5f, 1f);
                Matrix translate = Matrix.CreateTranslation(currentSize.x * 0.5f, currentSize.y * 0.5f, 0f);
                m_cachedWorldToScreenMatrix = viewProjectionMatrix * scale * translate;

                m_cachedRenderTargetSize = currentSize;
                m_matrixDirty = false;
            }
            return m_cachedWorldToScreenMatrix;
        }
    }

    public Vector2 GetWindowSize()
    {
        var pp = device.PresentationParameters;
        return new Vector2(pp.BackBufferWidth, pp.BackBufferHeight);
    }

    public Vector2 GetRenderTargetSize()
    {
        Microsoft.Xna.Framework.Graphics.RenderTargetBinding[] targets = device.GetRenderTargets();
        if (targets.Length > 0)
        {
            var rt = (Microsoft.Xna.Framework.Graphics.RenderTarget2D)targets[0].RenderTarget;
            return new Vector2(rt.Width, rt.Height);
        }
        else
        {
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