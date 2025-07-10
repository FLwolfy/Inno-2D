using Microsoft.Xna.Framework;

namespace Engine.Graphics.RenderPass;

/// <summary>
/// Clears the screen before rendering.
/// </summary>
public class ClearScreenPass : IRenderPass
{
    public RenderPassTag tag => RenderPassTag.ClearScreen;

    public void Render(RenderContext context)
    {
        context.graphicsDevice.Clear(Color.CornflowerBlue);
    }
}