using Inno.Core.Application;
using Inno.Core.Asset;
using Inno.Core.Events;
using Inno.Core.Layers;
using Inno.Core.Utility;
using Inno.Graphics;
using Inno.Graphics.Passes;
using Inno.Platform;
using Inno.Platform.Graphics;
using Inno.Platform.ImGui;
using Inno.Platform.Window;
using Inno.Runtime.RenderPasses;

namespace Inno.Runtime.Core;

public abstract class EngineCore
{
    private static readonly int DEFAULT_WINDOW_WIDTH = 1920;
    private static readonly int DEFAULT_WINDOW_HEIGHT = 1080;
    private static readonly bool DEFAULT_WINDOW_RESIZABLE = false;
    
    private readonly IWindow m_mainWindow;
    private readonly IGraphicsDevice m_graphicsDevice;
    private readonly IImGui m_imGui;
    
    private readonly Shell m_gameShell;
    private readonly LayerStack m_layerStack;
    private readonly RenderContext m_renderContext;
    
    // Render Resources
    private readonly Renderer2D m_renderer2D;
    private readonly RenderTargetPool m_renderTargetPool;
    private readonly RenderPassStack m_renderPassStack;
    
    protected EngineCore()
    {
        // Initialize platforms
        m_mainWindow = PlatformAPI.CreateWindow(new WindowInfo()
        {
            name = "Main Window",
            width = DEFAULT_WINDOW_WIDTH,
            height = DEFAULT_WINDOW_HEIGHT
        }, WindowBackend.Veldrid_Sdl2);
        m_mainWindow.resizable = DEFAULT_WINDOW_RESIZABLE;
        m_graphicsDevice = PlatformAPI.CreateGraphicsDevice(m_mainWindow, GraphicsBackend.Metal);
        m_imGui = PlatformAPI.CreateImGUI(m_mainWindow, m_graphicsDevice);
        
        // Initialize members
        m_gameShell = new Shell();
        m_layerStack = new LayerStack();
        m_renderPassStack = new RenderPassStack();
        
        // Initialize Render
        m_renderer2D = new Renderer2D(m_graphicsDevice);
        m_renderTargetPool = new RenderTargetPool(m_graphicsDevice);
        m_renderContext = new RenderContext
        (
            m_renderer2D,
            m_renderPassStack,
            m_renderTargetPool
        );
        
        // Initialization Callbacks
        m_gameShell.SetOnLoad(OnLoad);
        m_gameShell.SetOnSetup(OnSetup);
        
        // Update Callbacks
        m_gameShell.SetOnStep(OnStep);
        m_gameShell.SetOnEvent(OnEvent);
        m_gameShell.SetOnDraw(OnDraw);
        
        // Window Callback
        m_gameShell.SetOnClose(OnClose);
    }
    
    private void OnLoad()
    {
        // Asset Initialization
        AssetManager.SetRootDirectory("Assets");
        AssetRegistry.LoadFromDisk();
        
        // Renderer Resource Load
        m_renderer2D.LoadResources();
        m_renderPassStack.RegisterPass(new ClearScreenPass());
        m_renderPassStack.RegisterPass(new SpriteRenderPass());
    }

    private void OnSetup()
    {
        Setup(m_gameShell);
        RegisterLayers(m_layerStack);
        
        // Type Cache Initialization
        TypeCacheManager.Initialize();
    }

    private void OnStep()
    {
        // Layer Update
        m_layerStack.OnUpdate();
    }

    private void OnEvent(EventDispatcher dispatcher)
    {
        m_mainWindow.PumpEvents(dispatcher);
        dispatcher.Dispatch(m_layerStack.OnEvent);
    }

    private void OnDraw()
    {
        // Layer Render
        m_layerStack.OnRender();
        
        // Swap Buffers
        m_graphicsDevice.SwapBuffers();
    }

    private void OnClose()
    {
        AssetRegistry.SaveToDisk();
        
        // Dispose Resources
        m_renderer2D.Dispose();
        m_imGui.Dispose();
        m_renderTargetPool.Dispose();
        m_graphicsDevice.Dispose();
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
    protected abstract void Setup(Shell gameShell);

    /// <summary>
    /// Registers engine layers.
    /// </summary>
    protected abstract void RegisterLayers(LayerStack layerStack);
}
