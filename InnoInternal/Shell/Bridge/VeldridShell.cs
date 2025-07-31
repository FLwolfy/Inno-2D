using InnoInternal.Render.Bridge;
using InnoInternal.Render.Impl;

using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;
using InnoInternal.Shell.Impl;

namespace InnoInternal.Shell.Bridge;

internal sealed class VeldridShell : IGameShell
{
    private readonly Sdl2Window m_window;
    private readonly GraphicsDevice m_graphicsDevice;
    private readonly IRenderAPI m_renderAPI;

    private Action? m_onLoad;
    private Action? m_onSetup;
    private Action<float, float>? m_onStep;
    private Action<float>? m_onDraw;
    private Action? m_onClose;
    private Action<int, int>? m_onWindowSizeChanged;

    public VeldridShell()
    {
        WindowCreateInfo windowCI = new()
        {
            X = 100,
            Y = 100,
            WindowWidth = 1280,
            WindowHeight = 720,
            WindowTitle = "InnoEngine",
            WindowInitialState = WindowState.Normal,
        };

        m_window = VeldridStartup.CreateWindow(ref windowCI);
        GraphicsDeviceOptions gdOptions = new GraphicsDeviceOptions
        {
            Debug = false,
            SwapchainDepthFormat = null,
            SyncToVerticalBlank = true,
            ResourceBindingModel = ResourceBindingModel.Improved
        };
        
        m_graphicsDevice = VeldridStartup.CreateGraphicsDevice(m_window, gdOptions);
        m_renderAPI = new VeldridRenderAPI();
        m_renderAPI.Initialize(m_graphicsDevice);
        m_window.Resized += () =>
        {
            int w = m_window.Width;
            int h = m_window.Height;
            m_graphicsDevice.MainSwapchain.Resize((uint)w, (uint)h);
            m_onWindowSizeChanged?.Invoke(w, h);
        };
    }

    public void Run()
    {
        m_onLoad?.Invoke();
        m_onSetup?.Invoke();

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        double lastTime = stopwatch.Elapsed.TotalSeconds;

        while (m_window.Exists)
        {
            var now = stopwatch.Elapsed.TotalSeconds;
            float delta = (float)(now - lastTime);
            lastTime = now;

            InputSnapshot input = m_window.PumpEvents();
            if (!m_window.Exists) break;

            // Logics
            m_onStep?.Invoke((float)now, delta);
            
            // Draws
            m_onDraw?.Invoke(delta);
        }

        m_onClose?.Invoke();
    }

    public void Dispose()
    {
        m_graphicsDevice.Dispose();
        m_window.Close();
    }
    
    // API

    public void SetOnLoad(Action callback) => m_onLoad = callback;
    public void SetOnSetup(Action callback) => m_onSetup = callback;
    public void SetOnStep(Action<float, float> callback) => m_onStep = callback;
    public void SetOnDraw(Action<float> callback) => m_onDraw = callback;
    public void SetOnClose(Action callback) => m_onClose = callback;
    public void SetOnWindowSizeChanged(Action<int, int> callback) => m_onWindowSizeChanged = callback;

    public void SetWindowSize(int width, int height)
    {
        m_window.Width = width;
        m_window.Height = height;
        m_graphicsDevice.MainSwapchain.Resize((uint)width, (uint)height);
    }

    public void SetWindowResizable(bool enable)
    {
        m_window.Resizable = enable;
    }

    public object GetGraphicsDevice() => m_graphicsDevice;
    public object GetWindowHolder() => m_window;
    public IRenderAPI GetRenderAPI() => m_renderAPI;
}