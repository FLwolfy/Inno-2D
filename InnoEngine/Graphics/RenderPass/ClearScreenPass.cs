using InnoBase;
using InnoInternal.Render.Impl;

namespace InnoEngine.Graphics.RenderPass;

/// <summary>
/// Clears the screen before rendering.
/// </summary>
internal class ClearScreenPass : IRenderPass
{
    private static readonly Color CLEAR_COLOR = Color.CORNFLOWERBLUE;
    
    public RenderPassTag tag => RenderPassTag.ClearScreen;

    public void Render(IRenderAPI api)
    {
        api.command.Clear(CLEAR_COLOR);
    }
}