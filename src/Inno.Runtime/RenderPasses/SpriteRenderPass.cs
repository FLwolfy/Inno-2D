using Inno.Core.ECS;
using Inno.Core.Math;
using Inno.Graphics;
using Inno.Graphics.Pass;
using Inno.Runtime.Component;

namespace Inno.Runtime.RenderPasses;

/// <summary>
/// Renders all SpriteRenderers in the scene.
/// </summary>
public class SpriteRenderPass : RenderPass
{
    public override RenderPassTag orderTag => RenderPassTag.Geometry;

    public override void OnRender(RenderContext ctx)
    {
        var scene = SceneManager.GetActiveScene();
        if (scene == null) { return; }
        
        foreach (var spriteRenderer in scene.GetAllComponents<SpriteRenderer>())
        {
            if (!spriteRenderer.isActive) continue;
            
            // Solid Color Render
            if (spriteRenderer.sprite.texture == null)
            {
                var scale = Matrix.CreateScale(new Vector3(
                    spriteRenderer.sprite.size.x * spriteRenderer.transform.worldScale.x,
                    spriteRenderer.sprite.size.y * spriteRenderer.transform.worldScale.y,
                    1
                ));
                var rotation = Matrix.CreateFromQuaternion(spriteRenderer.transform.worldRotation);

                var depth =
                    (spriteRenderer.layerDepth +
                     (float)((Math.Tanh(spriteRenderer.transform.worldPosition.z / (float)SpriteRenderer.MAX_LAYER_DEPTH) + 1.0) / 2.0)) /
                    (SpriteRenderer.MAX_LAYER_DEPTH + 1);
                var translation = Matrix.CreateTranslation(new Vector3(
                    spriteRenderer.transform.worldPosition.x,
                    spriteRenderer.transform.worldPosition.y,
                    depth
                ));
                
                var transform = scale * rotation * translation;
                var color = spriteRenderer.color * spriteRenderer.opacity; // TODO: Handle alpha sequences
                
                Renderer2D.DrawQuad(ctx, transform, color);
            }
            
            // TODO: Support texture rendering logic here.
        }
    }

}