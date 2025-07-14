using InnoEngine.ECS;
using InnoEngine.ECS.Component;
using InnoEngine.Internal.Base;
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
        api.spriteBatch.Begin();

        var renderers = SceneManager.GetActiveScene()?.GetComponentManager().GetAll<SpriteRenderer>();
        if (renderers != null)
        {
            foreach (var r in renderers)
            {
                if (!r.isActive) continue;

                var cmd = r.GenerateRenderCommand();

                var sprite = cmd.sprite;
                var position = cmd.position;
                var scale = cmd.scale;

                float width = sprite.width * scale.x;
                float height = sprite.height * scale.y;

                var destinationRect = new Rect((int)position.x, (int)position.y, (int)width, (int)height);

                api.spriteBatch.DrawQuad(
                    destinationRect,
                    sprite.sourceRect,
                    sprite.texture.texture2DImpl,
                    cmd.color,
                    cmd.rotation,
                    cmd.depth,
                    cmd.origin
                );
            }
        }

        api.spriteBatch.End();
    }

}