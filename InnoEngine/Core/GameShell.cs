using InnoEngine.ECS;
using InnoEngine.Graphics;
using InnoEngine.Internal.Render.Bridge;
using InnoEngine.Internal.Shell;

namespace InnoEngine.Core;

public abstract class GameShell : MonoGameShell
{
    private readonly RenderSystem m_renderSystem = new RenderSystem();
    
    public sealed override void Load()
    {
        // Render Initialization
        // Resource.Manager.ResourceManager.Initialize(GraphicsDevice, m_contents);
        m_renderSystem.Initialize(new MonoGameRenderAPI(GraphicsDevice));
        m_renderSystem.LoadPasses();
        
        // Load Contents
        SetUp();
        
        // Start all the loaded scenes
        foreach (var scene in SceneManager.GetAllScenes()) { scene.Start(); }
    }

    public sealed override void Step(float totalTime, float deltaTime)
    {
        // Time Update
        Time.Update(totalTime, deltaTime);
        
        // Regular Update
        Step();
    }

    public sealed override void Draw(float deltaTime)
    {
        // Render Time Update
        Time.RenderUpdate(deltaTime);
        
        // Start Render
        m_renderSystem.Begin();
        
        var scene = SceneManager.GetActiveScene();
        if (scene == null) { return; }
        m_renderSystem.RenderPasses();
        
        // End Render
        m_renderSystem.End();
    }

    public abstract override void SetUp();
    public abstract void Step();
}
