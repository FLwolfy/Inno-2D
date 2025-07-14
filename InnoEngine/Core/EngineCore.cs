using InnoEngine.ECS;
using InnoEngine.Graphics;
using InnoEngine.Internal.Render.Bridge;
using InnoEngine.Internal.Render.Impl;
using InnoEngine.Internal.Resource.Bridge;
using InnoEngine.Internal.Resource.Impl;
using InnoEngine.Internal.Shell;
using InnoEngine.Resource;

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
        m_assetLoader.Initialize(((MonoGameShell)m_gameShell).GraphicsDevice);
        AssetManager.RegisterLoader(m_assetLoader);
        
        // Render Initialization
        m_renderAPI.Initialize(((MonoGameShell)m_gameShell).GraphicsDevice);
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
    public abstract void SetUp();
}
