
using InnoBase;
using InnoEngine.ECS;
using InnoEngine.ECS.Component;

namespace InnoEngine.Graphics.RenderPass;

/// <summary>
/// Renders all SpriteRenderers in the scene.
/// </summary>
internal class SpriteRenderPass : IRenderPass
{
    public RenderPassTag tag => RenderPassTag.Sprite;

    public void Render(RenderContext ctx)
    {
        var scene = SceneManager.GetActiveScene();
        if (scene == null) { return; }
        
        foreach (var spriteRenderer in scene.GetComponentManager().GetAll<SpriteRenderer>())
        {
            if (spriteRenderer.sprite.texture == null)
            {
                var scale = Matrix.CreateScale(new Vector3(
                    spriteRenderer.sprite.size.x * spriteRenderer.transform.worldScale.x,
                    spriteRenderer.sprite.size.y * spriteRenderer.transform.worldScale.y,
                    spriteRenderer.transform.worldScale.z
                ));
                var rotation = Matrix.CreateFromQuaternion(spriteRenderer.transform.worldRotation);
                var translation = Matrix.CreateTranslation(new Vector3(
                    spriteRenderer.transform.worldPosition.x,
                    spriteRenderer.transform.worldPosition.y,
                    spriteRenderer.transform.worldPosition.z
                ));
                
                var transform = scale * rotation * translation;
                var color = spriteRenderer.color * spriteRenderer.opacity;
                
                ctx.renderer.DrawQuad(transform, color);
            }
            
            // TODO: Support texture rendering logic here.
        }
    }

}