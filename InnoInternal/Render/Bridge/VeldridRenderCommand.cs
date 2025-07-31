using InnoBase;
using InnoInternal.Render.Impl;

using Veldrid;

namespace InnoInternal.Render.Bridge;

internal class VeldridRenderCommand : IRenderCommand
{
    public readonly GraphicsDevice graphicsDevice;
    public readonly CommandList commandList;

    public VeldridRenderCommand(GraphicsDevice device)
    {
        graphicsDevice = device;
        commandList = device.ResourceFactory.CreateCommandList();
    }

    public void Begin()
    {
        commandList.Begin();
    }

    public void End()
    {
        commandList.End();
        graphicsDevice.SubmitCommands(commandList);
        graphicsDevice.SwapBuffers();
    }

    public void DrawIndexed(uint indexCount, uint instanceCount = 1, uint indexStart = 0, int vertexOffset = 0,
        uint instanceStart = 0)
    {
        commandList.DrawIndexed(indexCount, instanceCount, indexStart, vertexOffset, instanceStart);
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
            commandList.SetFramebuffer(veldridTarget.rawFramebuffer);
        }
        else
        {
            var framebuffer = graphicsDevice.MainSwapchain.Framebuffer;
            commandList.SetFramebuffer(framebuffer);
        }
    }
}