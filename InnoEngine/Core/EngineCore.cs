using InnoEngine.Core.Layer;
using InnoEngine.Graphics;
using InnoEngine.Resource;
using InnoEngine.Utility;
using InnoInternal.ImGui.Impl;
using InnoInternal.Shell.Impl;

namespace InnoEngine.Core;

public abstract class EngineCore
{
    private static readonly int DEFAULT_WINDOW_WIDTH = 1920;
    private static readonly int DEFAULT_WINDOW_HEIGHT = 1080;
    private static readonly bool DEFAULT_WINDOW_RESIZABLE = false;
    
    private readonly IGameShell m_gameShell;
    private readonly LayerStack m_layerStack;
    private readonly RenderContext m_renderContext;
    
    protected EngineCore()
    {
        // Initialize members
        m_gameShell = IGameShell.CreateShell(IGameShell.ShellType.Veldrid);
        m_layerStack = new LayerStack();
        m_renderContext = new RenderContext
        (
            new Renderer2D(m_gameShell.GetGraphicsDevice()),
            IImGuiRenderer.CreateRenderer(IImGuiRenderer.ImGuiRendererType.Veldrid),
            new RenderPassController()
        );
        
        // Initialization Callbacks
        m_gameShell.SetWindowSize(DEFAULT_WINDOW_WIDTH,  DEFAULT_WINDOW_HEIGHT);
        m_gameShell.SetWindowResizable(DEFAULT_WINDOW_RESIZABLE);
        m_gameShell.SetOnLoad(OnLoad);
        m_gameShell.SetOnSetup(OnSetup);
        
        // Update Callbacks
        m_gameShell.SetOnStep(OnStep);
        m_gameShell.SetOnEvent(OnEvent);
        m_gameShell.SetOnDraw(OnDraw);
        
        // Window Callback
        m_gameShell.SetOnResize(m_renderContext.imGuiRenderer.OnWindowResize);
        m_gameShell.SetOnClose(AssetRegistry.SaveToDisk);
    }
    
    private void OnLoad()
    {
        // Asset Initialization
        AssetManager.SetRootDirectory("Assets");
        AssetRegistry.LoadFromDisk();
        
        // Renderer Resource Load
        m_renderContext.renderer.LoadResources();
        m_renderContext.imGuiRenderer.Initialize(m_gameShell.GetGraphicsDevice(), m_gameShell.GetWindowHolder());
        m_renderContext.passController.LoadPasses();
    }

    private void OnSetup()
    {
        Setup(m_gameShell);
        RegisterLayers(m_layerStack);
        
        // Type Cache Initialization
        TypeCacheManager.Initialize();
    }

    private void OnStep(float totalTime, float deltaTime)
    {
        // Time Update
        Time.Update(totalTime, deltaTime);
        
        // Layer Update
        m_layerStack.OnUpdate();
    }

    private void OnEvent(object e)
    {
        m_layerStack.OnEvent(e);
    }

    private void OnDraw(float deltaTime)
    {
        // Render Time Update
        Time.RenderUpdate(deltaTime);
        
        // Layer Render
        m_layerStack.OnRender(m_renderContext);
        
        // Swap Buffers
        m_gameShell.GetGraphicsDevice().SwapBuffers();
        m_renderContext.imGuiRenderer.SwapExtraImGuiWindows();
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
    protected abstract void Setup(IGameShell gameShell);

    /// <summary>
    /// Registers engine layers.
    /// </summary>
    protected abstract void RegisterLayers(LayerStack layerStack);
}
