using InnoEngine.ECS.Component;
using Microsoft.Xna.Framework.Graphics;

namespace InnoEngine.Graphics.RenderPass;

/// <summary>
/// Renders all SpriteRenderers in the scene.
/// </summary>
internal class SpriteRenderPass : IRenderPass
{
    public RenderPassTag tag => RenderPassTag.Sprite;

    public void Render(RenderContext context)
    {
        context.spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
        
        var renderers = context.gameScene.GetComponentManager().GetAll<SpriteRenderer>();
        foreach (var r in renderers)
        {
            if (!r.isActive) continue;
            var drawCommand = r.GenerateRenderCommand();
            
            context.spriteBatch.Draw(
                drawCommand.sprite.texture.native,
                drawCommand.position,
                drawCommand.sprite.sourceRect?.ToXnaRect(),
                drawCommand.color.ToXnaColor(),
                drawCommand.rotation,
                drawCommand.origin,
                drawCommand.scale,
                SpriteEffects.None,
                drawCommand.depth
            );
        }

        context.spriteBatch.End();
    }
}