using InnoBase;
using InnoEngine.ECS;
using InnoEngine.Graphics;

namespace InnoEngine.Core.Layer;

public class GameLayer() : Layer("GameLayer")
{
    public override void OnAttach()
    {
        // Start Scene Runtime
        SceneManager.BeginRuntime();
    }

    public override void OnEvent(Event e)
    {
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
        ctx.renderer.BeginFrame(camera.viewMatrix * camera.projectionMatrix, camera.aspectRatio, null);
        ctx.passController.RenderPasses(ctx);
        ctx.renderer.EndFrame();
    }
}