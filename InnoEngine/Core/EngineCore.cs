using InnoEngine.ECS;
using InnoEngine.Graphics;
using InnoEngine.Resource;
using InnoInternal.Render.Bridge;
using InnoInternal.Render.Impl;
using InnoInternal.Resource.Bridge;
using InnoInternal.Resource.Impl;
using InnoInternal.Shell.Bridge;
using InnoInternal.Shell.Impl;

namespace InnoEngine.Core;

public abstract class EngineCore
{
    private readonly IGameShell m_gameShell = new MonoGameShell();
    private readonly IRenderAPI m_renderAPI = new MonoGameRenderAPI();
    private readonly IAssetLoader m_assetLoader = new MonoGameAssetLoader();
    private readonly RenderSystem m_renderSystem = new();
    
    protected EngineCore()
    {
        // Initialization Callbacks
        m_gameShell.SetOnLoad(Load);
        m_gameShell.SetOnSetUp(() =>
        {
            SetUp();
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
        m_assetLoader.Initialize(m_gameShell.GetShellData());
        AssetManager.RegisterLoader(m_assetLoader);
        
        // Render Initialization
        m_renderAPI.Initialize(m_gameShell.GetShellData());
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
        
        var scene = SceneManager.GetActiveScene();
        if (scene == null) { return; }
        
        // Render Pipeline
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
    protected abstract void SetUp();
}
