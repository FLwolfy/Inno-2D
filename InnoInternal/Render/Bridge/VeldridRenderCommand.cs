using InnoBase;
using InnoInternal.Render.Impl;
using Veldrid;

namespace InnoInternal.Render.Bridge;

internal class VeldridRenderCommand : IRenderCommand
{
    private readonly GraphicsDevice m_graphicsDevice;
    
    public readonly CommandList commandList;
    public Framebuffer currentFrameBuffer { get; private set; }

    public VeldridRenderCommand(GraphicsDevice device)
    {
        m_graphicsDevice = device;
        currentFrameBuffer = device.MainSwapchain.Framebuffer;
        var factory = device.ResourceFactory;
        commandList = factory.CreateCommandList();
    }

    public void BeginFrame()
    {
        commandList.Begin();
    }

    public void EndFrame()
    {
        commandList.End();
        m_graphicsDevice.SubmitCommands(commandList);
        m_graphicsDevice.SwapBuffers();
    }

    public void Clear(Color color)
    {
        commandList.ClearColorTarget(0, new RgbaFloat(
            color.r,
            color.g,
            color.b,
            color.a));
    }

    public void SetRenderTarget(IRenderTarget? target)
    {
        if (target is VeldridRenderTarget veldridTarget)
        {
            currentFrameBuffer = veldridTarget.rawFramebuffer;
            commandList.SetFramebuffer(veldridTarget.rawFramebuffer);
            commandList.SetScissorRect(0,0, 0, veldridTarget.width, veldridTarget.height);
        }
        else
        {
            var framebuffer = m_graphicsDevice.MainSwapchain.Framebuffer;
            currentFrameBuffer = framebuffer;
            commandList.SetFramebuffer(framebuffer);
            commandList.SetScissorRect(0,0, 0, framebuffer.Width, framebuffer.Height);
        }
    }

    public IRenderTarget CreateRenderTarget(uint width, uint height)
    {
        return new VeldridRenderTarget(m_graphicsDevice, width, height);
    }
}