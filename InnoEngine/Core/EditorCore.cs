using InnoEngine.ECS;
using InnoEngine.Graphics;
using InnoEngine.Internal.Render.Bridge;
using InnoEngine.Internal.Render.Impl;
using InnoEngine.Internal.Resource.Bridge;
using InnoEngine.Internal.Resource.Impl;
using InnoEngine.Internal.Shell;
using InnoEngine.Resource;

namespace InnoEngine.Core;

/// <summary>
/// Core class for launching the editor mode of the engine.
/// Handles asset loading, render pipeline, and editor-specific setup.
/// </summary>
public abstract class EditorCore
{
    private readonly IGameShell m_gameShell = new MonoGameShell();
    private readonly IRenderAPI m_renderAPI = new MonoGameRenderAPI();
    private readonly IAssetLoader m_assetLoader = new MonoGameAssetLoader();
    private readonly RenderSystem m_renderSystem = new();

    protected EditorCore()
    {
        // Initialization Callbacks
        m_gameShell.SetOnLoad(Load);
        m_gameShell.SetOnSetUp(() =>
        {
            SetUp();
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
        m_assetLoader.Initialize(((MonoGameShell)m_gameShell).GraphicsDevice);
        AssetManager.RegisterLoader(m_assetLoader);

        // Render Init
        m_renderAPI.Initialize(((MonoGameShell)m_gameShell).GraphicsDevice);
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

        // Optional: Scene rendering
        var scene = SceneManager.GetActiveScene();
        if (scene != null)
        {
            m_renderSystem.Begin();
            m_renderSystem.RenderPasses();
            m_renderSystem.End();
        }

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
    /// Called during setup (after load), use to construct panels or layout.
    /// </summary>
    public abstract void SetUp();

    /// <summary>
    /// Called every frame during editor mode.
    /// </summary>
    public abstract void OnEditorUpdate(float totalTime, float deltaTime);

    /// <summary>
    /// Called every frame to draw ImGui panels.
    /// </summary>
    public abstract void OnEditorGUI(float deltaTime);

    /// <summary>
    /// Called once after setup, editor is now fully ready.
    /// </summary>
    public virtual void OnEditorStart() { }
}
