using Color = InnoEngine.Base.Color;

namespace InnoEngine.Graphics.RenderPass;

/// <summary>
/// Clears the screen before rendering.
/// </summary>
internal class ClearScreenPass : IRenderPass
{
    private static readonly Color CLEAR_COLOR = Color.CORNFLOWERBLUE;
    
    public RenderPassTag tag => RenderPassTag.ClearScreen;

    public void Render(RenderContext context)
    {
        context.graphicsDevice.Clear(CLEAR_COLOR.ToXnaColor());
    }
}