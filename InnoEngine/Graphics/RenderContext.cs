using InnoEngine.Graphics.RenderPass;
using InnoInternal.ImGui.Impl;

namespace InnoEngine.Graphics;

public class RenderContext
{
    public Renderer2D renderer2D { get; }
    public IImGuiRenderer imGuiRenderer { get; }
    public RenderPassController passController { get; }
    public RenderTargetPool targetPool { get; }
    
    internal RenderContext(Renderer2D renderer2D, IImGuiRenderer imGuiRenderer, RenderPassController passController, RenderTargetPool targetPool)
    {
        this.renderer2D = renderer2D;
        this.imGuiRenderer = imGuiRenderer;
        this.passController = passController;
        this.targetPool = targetPool;
    }
}