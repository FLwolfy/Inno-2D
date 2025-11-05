using InnoBase;
using InnoEngine.ECS;
using InnoEngine.Graphics;

namespace InnoEngine.Core.Layer;

public class GameLayer : Layer
{
    public GameLayer() : base("GameLayer") { }

    public override void OnAttach()
    {
        SceneManager.BeginRuntime();
    }

    public override void OnUpdate()
    {
        SceneManager.GetActiveScene()?.Update();
    }

    public override void OnRender(RenderContext ctx)
    {
        // Render Test
        ctx.renderer.BeginFrame();
        ctx.renderer.DrawQuad(new Vector2(0, 0), new Vector2(1, 1), Color.RED);
        ctx.renderer.EndFrame();
        
        // Get Scene and Camera
        var scene = SceneManager.GetActiveScene();
        if (scene == null) { return; }
        var camera = scene.GetCameraManager().mainCamera;
        if (camera == null) { return; }
        
        // Render Pipeline
        // m_renderAPI.renderContext.viewMatrix = camera.viewMatrix;
        // m_renderAPI.renderContext.projectionMatrix = camera.projectionMatrix;
        // m_renderSystem.Begin();
        // m_renderSystem.RenderPasses();
        // m_renderSystem.End();
    }
}