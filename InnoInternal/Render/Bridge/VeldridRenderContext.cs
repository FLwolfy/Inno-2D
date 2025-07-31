using InnoBase;
using InnoInternal.Render.Impl;

namespace InnoInternal.Render.Bridge;

internal class VeldridRenderContext : IRenderContext
{
    private VeldridRenderCommand m_command = null!;
    private IRenderTarget? m_renderTarget;
    
    public Matrix viewMatrix { get; set; } = Matrix.identity;
    public Matrix projectionMatrix { get; set; } = Matrix.identity;
    
    public void Initialize(IRenderCommand command)
    {
        m_command = (VeldridRenderCommand) command;
    }
    
    public Vector2 GetWindowSize()
    {
        var frameBuffer = m_command.graphicsDevice.SwapchainFramebuffer;
        return new Vector2(frameBuffer.Width, frameBuffer.Height);
    }

    public void SetRenderTarget(IRenderTarget? target)
    {
        m_renderTarget = target;
        m_command.SetRenderTarget(target);
    }

    public IRenderTarget? GetRenderTarget()
    {
        return m_renderTarget;
    }

    public IRenderTarget CreateRenderTarget(uint width, uint height)
    {
        return new VeldridRenderTarget(m_command.graphicsDevice, width, height);
    }
}
