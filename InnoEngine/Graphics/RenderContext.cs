using InnoBase;
using InnoInternal.ImGui.Impl;
using InnoInternal.Render.Impl;

namespace InnoEngine.Graphics;

public class RenderContext(IGraphicsDevice graphicsDevice, Renderer2D renderer, IImGuiRenderer imGuiRenderer, RenderPassController passController)
{
    public IGraphicsDevice graphicsDevice { get; } = graphicsDevice;
    public Renderer2D renderer { get; } = renderer;
    public IImGuiRenderer imGuiRenderer { get; } = imGuiRenderer;
    public RenderPassController passController { get; } = passController;

    public Matrix viewProjectionMatrix => renderer.viewProjection;
}