using InnoBase.Graphics;
using InnoEngine.ECS;
using InnoEngine.Graphics;
using InnoInternal.Render.Impl;

namespace InnoEngine.Core.Layer;

public class GameLayer() : Layer("GameLayer")
{
    public override void OnAttach()
    {
        // Start Scene Runtime
        SceneManager.BeginRuntime();
    }

    public override void OnUpdate()
    {
        SceneManager.GetActiveScene()?.Update();
    }

    public override void OnRender(RenderContext ctx)
    {
        // Get Camera
        var camera = SceneManager.GetActiveScene()?.GetCameraManager().mainCamera;
        if (camera == null) { return; }
        
        // Render Pipeline
        EnsureSceneRenderTarget(ctx);
        ctx.renderer2D.BeginFrame(camera.viewMatrix * camera.projectionMatrix, camera.aspectRatio, ctx.targetPool.GetMain()); // TODO: Use Renderer2D Blit
        ctx.passController.RenderPasses(ctx);
        ctx.renderer2D.EndFrame();
    }
    
    private void EnsureSceneRenderTarget(RenderContext ctx)
    {
        if (ctx.targetPool.Get("scene") == null)
        {
            var renderTexDesc = new TextureDescription
            {
                format = PixelFormat.B8_G8_R8_A8_UNorm,
                usage = TextureUsage.RenderTarget | TextureUsage.Sampled,
                dimension = TextureDimension.Texture2D
            };
            
            var depthTexDesc = new TextureDescription
            {
                format = PixelFormat.D32_Float_S8_UInt,
                usage = TextureUsage.DepthStencil,
                dimension = TextureDimension.Texture2D
            };
            
            var renderTargetDesc = new FrameBufferDescription
            {
                depthAttachmentDescription = depthTexDesc,
                colorAttachmentDescriptions = [renderTexDesc]
            };
            
            ctx.targetPool.Create("scene", renderTargetDesc);
        }
    }
}