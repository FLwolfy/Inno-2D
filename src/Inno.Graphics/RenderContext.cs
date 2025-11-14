using Inno.Graphics.Passes;

namespace Inno.Graphics;

public class RenderContext
{
    public Renderer2D renderer2D { get; }
    public RenderPassStack passStack { get; }
    public RenderTargetPool targetPool { get; }
    
    public RenderContext(Renderer2D renderer2D, RenderPassStack passStack, RenderTargetPool targetPool)
    {
        this.renderer2D = renderer2D;
        this.passStack = passStack;
        this.targetPool = targetPool;
    }
}