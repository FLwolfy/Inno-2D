using System.Diagnostics;
using InnoBase;
using InnoBase.Event;
using InnoInternal.Render.Bridge;
using InnoInternal.Render.Impl;
using InnoInternal.Shell.Impl;

using Veldrid;
using Veldrid.StartupUtilities;

namespace InnoInternal.Shell.Bridge;

internal class VeldridShell : IGameShell
{
    private Action? m_onLoad;
    private Action? m_onSetup;
    private Action<float, float>? m_onStep;
    private Action<float>? m_onDraw;
    private Action? m_onClose;

    private readonly VeldridSdl2Window m_windowVeldrid;
    private readonly VeldridGraphicsDevice m_graphicsDeviceVeldrid;
    private readonly Stopwatch m_timer = new();
    
    private double m_lastTime;
    
    public void SetWindowSize(int width, int height) => (m_windowVeldrid.width, m_windowVeldrid.height) = (width, height);
    public void SetWindowResizable(bool resizable) => m_windowVeldrid.resizable = resizable;

    public void SetOnLoad(Action onLoad) => m_onLoad = onLoad;
    public void SetOnSetup(Action onSetup) => m_onSetup = onSetup;
    public void SetOnStep(Action<float, float> onStep) => m_onStep = onStep;
    public void SetOnEvent(Action<Event> onEvent) => m_windowVeldrid.OnEvent += onEvent;
    public void SetOnDraw(Action<float> onDraw) => m_onDraw = onDraw;
    public void SetOnClose(Action onClose) => m_onClose = onClose;

    public VeldridShell()
    {
        var windowCi = new WindowCreateInfo
        {
            WindowWidth = 1280,
            WindowHeight = 720,
            WindowTitle = "InnoEngine",
            WindowInitialState = WindowState.Normal
        };
        
        var window = VeldridStartup.CreateWindow(ref windowCi);
        m_windowVeldrid = new VeldridSdl2Window(window);

        var deviceOptions = new GraphicsDeviceOptions(
            debug: true,
            null,
            syncToVerticalBlank: false,
            resourceBindingModel: ResourceBindingModel.Improved,
            preferDepthRangeZeroToOne: true,
            preferStandardClipSpaceYDirection: true
        );

        var graphicsDevice = VeldridStartup.CreateGraphicsDevice(window, deviceOptions);
        m_graphicsDeviceVeldrid = new VeldridGraphicsDevice(graphicsDevice);
        
        window.Resized += () =>
        {
            m_graphicsDeviceVeldrid.swapchainFrameBuffer.Resize(window.Width, window.Height);
        };
    }

    public void Run()
    {
        m_onLoad?.Invoke();
        m_onSetup?.Invoke();

        m_timer.Start();
        m_lastTime = 0.0;

        while (m_windowVeldrid.exists)
        {
            double now = m_timer.Elapsed.TotalSeconds;
            float delta = (float)(now - m_lastTime);
            m_lastTime = now;

            // Inputs
            m_windowVeldrid.PumpEvents();
            
            // Logic Step
            m_onStep?.Invoke((float)now, delta);
            
            // Render
            m_onDraw?.Invoke(delta);
        }

        m_onClose?.Invoke();
        m_graphicsDeviceVeldrid.Dispose();
    }

    public IGraphicsDevice GetGraphicsDevice() => m_graphicsDeviceVeldrid;
    public IWindow GetWindow() => m_windowVeldrid;
}
