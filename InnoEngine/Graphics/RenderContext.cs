using InnoBase;

namespace InnoEngine.Graphics;

public class RenderContext(Renderer2D renderer, RenderPassController passController)
{
    public Renderer2D renderer { get; } = renderer;
    public RenderPassController passController { get; } = passController;

    public Matrix viewProjectionMatrix => renderer.viewProjection;
}