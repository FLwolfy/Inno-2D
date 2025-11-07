using InnoEngine.ECS;
using InnoEngine.Graphics;

namespace InnoEngine.Core.Layer;

public class GameLayer : Layer
{
    public GameLayer() : base("GameLayer") { }

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
        // Get Scene and Camera
        var scene = SceneManager.GetActiveScene();
        if (scene == null) { return; }
        var camera = scene.GetCameraManager().mainCamera;
        if (camera == null) { return; }
        
        // Render Pipeline
        ctx.renderer.BeginFrame(camera.viewMatrix * camera.projectionMatrix);
        ctx.passController.RenderPasses(ctx);
        ctx.renderer.EndFrame();
    }
}