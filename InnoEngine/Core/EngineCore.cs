using InnoEngine.Core.Layer;
using InnoEngine.Graphics;
using InnoEngine.Resource;
using InnoEngine.Utility;
using InnoInternal.Shell.Impl;

namespace InnoEngine.Core;

public abstract class EngineCore
{
    private static readonly int WINDOW_WIDTH = 1920;
    private static readonly int WINDOW_HEIGHT = 1080;
    
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
            new RenderPassController()
        );
        
        // Initialization Callbacks
        m_gameShell.SetWindowSize(WINDOW_WIDTH,  WINDOW_HEIGHT);
        m_gameShell.SetWindowResizable(true);
        m_gameShell.SetOnLoad(OnLoad);
        m_gameShell.SetOnSetup(OnSetup);
        
        // Update Callbacks
        m_gameShell.SetOnStep(OnStep);
        m_gameShell.SetOnEvent(OnEvent);
        m_gameShell.SetOnDraw(OnDraw);
        
        // Close Callback
        m_gameShell.SetOnClose(AssetRegistry.SaveToDisk);
    }
    
    private void OnLoad()
    {
        // Resource Initialization
        AssetManager.SetRootDirectory("Assets");
        AssetRegistry.LoadFromDisk();
        
        // Renderer Initialization
        m_renderContext.renderer.LoadResources();
        m_renderContext.passController.LoadPasses();
    }

    private void OnSetup()
    {
        TypeCacheManager.Initialize();
        Setup();
        RegisterLayers(m_layerStack);
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

    /// <summary>
    /// Registers engine layers.
    /// </summary>
    protected abstract void RegisterLayers(LayerStack layerStack);
}
