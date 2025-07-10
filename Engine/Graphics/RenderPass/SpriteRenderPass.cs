using Engine.ECS;
using Engine.ECS.Components;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Graphics.RenderPass;

/// <summary>
/// Renders all SpriteRenderers in the scene.
/// </summary>
public class SpriteRenderPass : IRenderPass
{
    public RenderPassTag tag => RenderPassTag.World;

    public void Render(RenderContext context)
    {
        var scene = SceneManager.GetActiveScene();

        using (new RenderBatchScope(context.spriteBatch, SpriteSortMode.FrontToBack))
        {
            var renderers = scene.GetComponentManager().GetAll<SpriteRenderer>();
            foreach (var r in renderers.OrderBy(r => r.layerDepth).ThenBy(r => r.transform.worldPosition.Z))
            {
                if (!r.isActive) continue;
                r.Render(context.spriteBatch);
            }
        }
    }
}