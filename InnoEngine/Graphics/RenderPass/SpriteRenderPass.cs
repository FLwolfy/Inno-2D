using InnoBase;
using InnoEngine.ECS;
using InnoEngine.ECS.Component;
using InnoInternal.Render.Impl;

namespace InnoEngine.Graphics.RenderPass;

/// <summary>
/// Renders all SpriteRenderers in the scene.
/// </summary>
internal class SpriteRenderPass : IRenderPass
{
    public RenderPassTag tag => RenderPassTag.Sprite;

    public void Render(IRenderAPI api)
    {
        // TODO
    }

}