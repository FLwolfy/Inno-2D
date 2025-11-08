using InnoBase;
using InnoInternal.ImGui.Impl;

namespace InnoEngine.Graphics;

public class RenderContext(Renderer2D renderer, IImGuiRenderer imGuiRenderer, RenderPassController passController)
{
    public Renderer2D renderer { get; } = renderer;

    public IImGuiRenderer imGuiRenderer { get; } = imGuiRenderer;
    public RenderPassController passController { get; } = passController;

    public Matrix viewProjectionMatrix => renderer.viewProjection;
}