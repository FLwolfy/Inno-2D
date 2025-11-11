using InnoInternal.ImGui.Impl;

namespace InnoEngine.Graphics;

public class RenderContext(Renderer2D renderer2D, IImGuiRenderer imGuiRenderer, RenderPassController passController, RenderTargetPool targetPool)
{
    public Renderer2D renderer2D { get; } = renderer2D;
    public RenderPassController passController { get; } = passController;
    public RenderTargetPool targetPool { get; } = targetPool;
    public IImGuiRenderer imGuiRenderer { get; } = imGuiRenderer;
}