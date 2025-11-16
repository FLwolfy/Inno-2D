using Inno.Core.Math;
using Inno.Graphics;
using Inno.Graphics.Pass;

namespace Inno.Runtime.RenderPasses;

/// <summary>
/// Clears the screen before rendering.
/// </summary>
public class ClearScreenPass : RenderPass
{
    private static readonly Color CLEAR_COLOR = Color.CORNFLOWERBLUE;
    
    public override RenderPassTag orderTag => RenderPassTag.ClearScreen;

    public override void OnRender(RenderContext ctx)
    {
        Renderer2D.ClearColor(ctx, CLEAR_COLOR);
    }
}