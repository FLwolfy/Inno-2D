using InnoBase;
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

/// <summary>
/// Core class for launching the editor mode of the engine.
/// Handles asset loading, render pipeline, and editor-specific setup.
/// </summary>
public abstract class EditorCore
{
    private const int c_windowWidth = 1280;
    private const int c_windowHeight = 700;
    
    private readonly IGameShell m_gameShell = new MonoGameShell();
    private readonly IRenderAPI m_renderAPI = new MonoGameRenderAPI();
    private readonly IAssetLoader m_assetLoader = new MonoGameAssetLoader();
    private readonly RenderSystem m_renderSystem = new();

    protected EditorCore()
    {
        // Initialization Callbacks
        m_gameShell.SetWindowSize(c_windowWidth,  c_windowHeight);
        m_gameShell.SetWindowResizable(true);
        m_gameShell.SetOnLoad(Load);
        m_gameShell.SetOnSetup(() =>
        {
            Setup();
            OnEditorStart();
        });

        // Update Callbacks
        m_gameShell.SetOnStep(Step);
        m_gameShell.SetOnDraw(Draw);

        // Shutdown Callback
        m_gameShell.SetOnClose(AssetRegistry.SaveToDisk);
    }

    private void Load()
    {
        // Asset Load
        AssetManager.SetRootDirectory("Assets");
        AssetRegistry.LoadFromDisk();
        AssetManager.RegisterLoader(m_assetLoader);

        // Render Init
        m_renderAPI.Initialize(m_gameShell.GetGraphicsDevice());
        m_renderSystem.Initialize(m_renderAPI);
        m_renderSystem.LoadPasses();
    }

    private void Step(float totalTime, float deltaTime)
    {
        Time.Update(totalTime, deltaTime);

        // Editor Logic Step
        OnEditorUpdate(totalTime, deltaTime);
    }

    private void Draw(float deltaTime)
    {
        Time.RenderUpdate(deltaTime);
        
        // ImGui GUI Draw
        OnEditorGUI(deltaTime);
    }

    /// <summary>
    /// Runs the editor application.
    /// </summary>
    public void Run()
    {
        m_gameShell.Run();
    }

    /// <summary>
    /// Try to render the scene. This automatically checks whether there is an active scene.
    /// </summary>
    protected bool TryRenderScene(Matrix viewMatrix, Matrix projectionMatrix)
    {
        var scene = SceneManager.GetActiveScene();
        if (scene == null)  { return false; }
    
        m_renderAPI.context.viewMatrix = viewMatrix;
        m_renderAPI.context.projectionMatrix = projectionMatrix;
        
        m_renderSystem.Begin();
        m_renderSystem.RenderPasses();
        m_renderSystem.End();
        
        return true;
    }
    
    /// <summary>
    /// Gets the window holder of the game shell.
    /// </summary>
    protected object GetWindowHolder() => m_gameShell.GetWindowHolder();
    
    /// <summary>
    /// Gets the render API of the editor core.
    /// </summary>
    protected object GetRenderAPI() => m_renderAPI;

    /// <summary>
    /// Called during setup (after load), use to construct panels or layout.
    /// </summary>
    protected abstract void Setup();

    /// <summary>
    /// Called every frame during editor mode.
    /// </summary>
    protected abstract void OnEditorUpdate(float totalTime, float deltaTime);

    /// <summary>
    /// Called every frame to draw ImGui panels.
    /// </summary>
    protected abstract void OnEditorGUI(float deltaTime);

    /// <summary>
    /// Called once after setup, editor is now fully ready.
    /// </summary>
    protected virtual void OnEditorStart() { }
}
