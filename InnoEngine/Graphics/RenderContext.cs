using InnoBase;
using InnoInternal.ImGui.Impl;
using InnoInternal.Render.Impl;

namespace InnoEngine.Graphics;

public class RenderContext(IGraphicsDevice graphicsDevice, IFrameBuffer renderTarget, Renderer2D renderer, IImGuiRenderer imGuiRenderer, RenderPassController passController)
{
    public IGraphicsDevice graphicsDevice { get; } = graphicsDevice;
    public IFrameBuffer renderTarget { get; } = renderTarget;
    public Renderer2D renderer { get; } = renderer;
    public IImGuiRenderer imGuiRenderer { get; } = imGuiRenderer;
    public RenderPassController passController { get; } = passController;
}