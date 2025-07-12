using InnoEngine.ECS.Component;
using InnoEngine.Internal.Render.Impl;

namespace InnoEngine.Graphics.RenderPass;

/// <summary>
/// Renders all SpriteRenderers in the scene.
/// </summary>
internal class SpriteRenderPass : IRenderPass
{
    public RenderPassTag tag => RenderPassTag.Sprite;

    public void Render(IRenderAPI api)
    {
        // TODO: allow blendAlpha and sortMode, currently FRONT_TO_BACK and ALPHA_BLEND are hardcoded
        api.spriteBatch.Begin();
        
        var renderers = context.gameScene.GetComponentManager().GetAll<SpriteRenderer>();
        foreach (var r in renderers)
        {
            if (!r.isActive) continue;
            var drawCommand = r.GenerateRenderCommand();
            
            api.spriteBatch.DrawQuad(
                drawCommand.sprite.texture.native,
                drawCommand.position,
                drawCommand.sprite.sourceRect?.ToXnaRect(),
                drawCommand.color.ToXnaColor(),
                drawCommand.rotation,
                drawCommand.origin,
                drawCommand.scale,
                drawCommand.depth
            );
        }

        api.spriteBatch.End();
    }
}