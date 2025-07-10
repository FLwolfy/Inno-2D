using InnoEngine.ECS.Component;
using Microsoft.Xna.Framework.Graphics;

namespace InnoEngine.Graphics.RenderPass;

/// <summary>
/// Renders all SpriteRenderers in the scene.
/// </summary>
public class SpriteRenderPass : IRenderPass
{
    public RenderPassTag tag => RenderPassTag.Sprite;

    public void Render(RenderContext context)
    {
        using (new RenderBatchScope(context.spriteBatch, SpriteSortMode.FrontToBack))
        {
            var renderers = context.gameScene.GetComponentManager().GetAll<SpriteRenderer>();
            foreach (var r in renderers)
            {
                if (!r.isActive) continue;
                r.Render(context.spriteBatch);
            }
        }
    }
}