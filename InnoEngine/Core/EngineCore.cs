using InnoEngine.ECS;
using InnoEngine.Graphics;
using InnoEngine.Resource;
using InnoEngine.Utility;
using InnoInternal.Render.Bridge;
using InnoInternal.Render.Impl;
using InnoInternal.Resource.Bridge;
using InnoInternal.Resource.Impl;
using InnoInternal.Shell.Impl;

namespace InnoEngine.Core;

public abstract class EngineCore
{
    private static readonly int WINDOW_WIDTH = 1280;
    private static readonly int WINDOW_HEIGHT = 720;
    
    private readonly IGameShell m_gameShell = new MonoGameShell();
    private readonly IRenderAPI m_renderAPI = new VeldridRenderAPI();
    private readonly IAssetLoader m_assetLoader = new MonoGameAssetLoader();
    private readonly RenderSystem m_renderSystem = new();
    
    protected EngineCore()
    {
        // Initialization Callbacks
        m_gameShell.SetWindowSize(WINDOW_WIDTH,  WINDOW_HEIGHT);
        m_gameShell.SetWindowResizable(true);
        m_gameShell.SetOnLoad(Load);
        m_gameShell.SetOnSetup(() =>
        {
            Setup();
            TypeCacheManager.Initialize();
            SceneManager.BeginRuntime();
        });
        
        // Update Callbacks
        m_gameShell.SetOnStep(Step);
        m_gameShell.SetOnDraw(Draw);
        
        // Close Callback
        m_gameShell.SetOnClose(AssetRegistry.SaveToDisk);
    }
    
    private void Load()
    {
        // Resource Initialization
        AssetManager.SetRootDirectory("Assets");
        AssetRegistry.LoadFromDisk();
        AssetManager.RegisterLoader(m_assetLoader);
        
        // Render Initialization
        m_renderAPI.Initialize(m_gameShell.GetGraphicsDevice());
        m_renderSystem.Initialize(m_renderAPI);
        m_renderSystem.LoadPasses();
    }

    private void Step(float totalTime, float deltaTime)
    {
        // Time Update
        Time.Update(totalTime, deltaTime);
        
        // Scene Update
        SceneManager.GetActiveScene()?.Update();
    }

    private void Draw(float deltaTime)
    {
        // Render Time Update
        Time.RenderUpdate(deltaTime);
        
        // Get Scene and Camera
        var scene = SceneManager.GetActiveScene();
        if (scene == null) { return; }
        var camera = scene.GetCameraManager().mainCamera;
        if (camera == null) { return; }
        
        // Render Pipeline
        m_renderAPI.context.viewMatrix = camera.viewMatrix;
        m_renderAPI.context.projectionMatrix = camera.projectionMatrix;
        m_renderSystem.Begin();
        m_renderSystem.RenderPasses();
        m_renderSystem.End();
    }
    
    /// <summary>
    /// Runs the game shell, starting the main loop of the engine.
    /// </summary>
    public void Run()
    {
        m_gameShell.Run();
    }

    /// <summary>
    /// Sets up the engine core.
    /// </summary>
    protected abstract void Setup();
}
