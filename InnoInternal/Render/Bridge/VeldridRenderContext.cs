using InnoBase;
using InnoInternal.Render.Impl;

using Veldrid;

namespace InnoInternal.Render.Bridge;

internal class VeldridRenderContext : IRenderContext
{
    private readonly GraphicsDevice m_graphicsDevice;
    private readonly VeldridRenderCommand m_command;

    private Matrix m_cachedViewMatrix = Matrix.identity;
    private Matrix m_cachedProjectionMatrix = Matrix.identity;
    private Matrix m_cachedWorldToScreenMatrix = Matrix.identity;
    private Vector2 m_cachedRenderTargetSize = Vector2.ZERO;

    private bool m_matrixDirty = true;

    public VeldridRenderContext(GraphicsDevice device, VeldridRenderCommand command)
    {
        m_graphicsDevice = device;
        m_command = command;
    }

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
            Vector2 currentSize = GetCurrentRenderTargetSize();
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
        var swapchain = m_graphicsDevice.MainSwapchain;
        return new Vector2(swapchain.Framebuffer.Width, swapchain.Framebuffer.Height);
    }

    public Vector2 GetCurrentRenderTargetSize()
    {
        return new Vector2(m_command.currentFrameBuffer.Width, m_command.currentFrameBuffer.Height);
    }
}
