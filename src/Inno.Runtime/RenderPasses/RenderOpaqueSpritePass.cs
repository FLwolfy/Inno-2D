using Inno.Core.ECS;
using Inno.Core.Math;
using Inno.Graphics;
using Inno.Graphics.Pass;
using Inno.Runtime.Component;

namespace Inno.Runtime.RenderPasses;

public class RenderOpaqueSpritePass : RenderPass
{
    public override RenderPassTag orderTag => RenderPassTag.Geometry;

    public override void OnRender(RenderContext ctx)
    {
        var scene = SceneManager.GetActiveScene();
        if (scene == null) return;

        foreach (var sr in scene.GetAllComponents<SpriteRenderer>().Where(sr => sr.isActive && sr.opacity >= 1f && sr.color.a >= 1f))
        {
            var t = Matrix.CreateScale(new Vector3(sr.sprite.size.x * sr.transform.worldScale.x, sr.sprite.size.y * sr.transform.worldScale.y, 1)) *
                    Matrix.CreateFromQuaternion(sr.transform.worldRotation) *
                    Matrix.CreateTranslation(new Vector3(sr.transform.worldPosition.x, sr.transform.worldPosition.y, sr.layerDepth));

            Renderer2D.DrawQuad(ctx, t, sr.color * sr.opacity);
        }
    }
}