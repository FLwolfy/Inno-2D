using System.Diagnostics;
using InnoInternal.Render.Bridge;
using InnoInternal.Render.Impl;
using InnoInternal.Shell.Impl;

using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;


namespace InnoInternal.Shell.Bridge;

internal class VeldridShell : IGameShell
{
    private Action? m_onLoad;
    private Action? m_onSetup;
    private Action<float, float>? m_onStep;
    private Action<object>? m_onEvent;
    private Action<float>? m_onDraw;
    private Action? m_onClose;
    private Action<int, int>? m_onResize;

    private readonly Sdl2Window m_window;
    private readonly GraphicsDevice m_graphicsDevice;
    private readonly VeldridGraphicsDevice m_graphicsDeviceVeldrid;
    private readonly Stopwatch m_timer = new();
    
    private double m_lastTime;
    
    public void SetWindowSize(int width, int height) => (m_window.Width, m_window.Height) = (width, height);
    public void SetWindowResizable(bool resizable) => m_window.Resizable = resizable;

    public void SetOnLoad(Action onLoad) => m_onLoad = onLoad;
    public void SetOnSetup(Action onSetup) => m_onSetup = onSetup;
    public void SetOnStep(Action<float, float> onStep) => m_onStep = onStep;
    public void SetOnEvent(Action<object> onEvent) => m_onEvent = onEvent;
    public void SetOnDraw(Action<float> onDraw) => m_onDraw = onDraw;
    public void SetOnClose(Action onClose) => m_onClose = onClose;
    public void SetOnResize(Action<int, int> onResize) => m_onResize = onResize;

    public VeldridShell()
    {
        var windowCi = new WindowCreateInfo
        {
            WindowWidth = 1280,
            WindowHeight = 720,
            WindowTitle = "ImGui Test",
            WindowInitialState = WindowState.Normal
        };
        
        m_window = VeldridStartup.CreateWindow(ref windowCi);
        m_graphicsDevice = VeldridStartup.CreateGraphicsDevice(m_window);
        m_graphicsDeviceVeldrid = new VeldridGraphicsDevice(m_graphicsDevice);
        
        m_window.Resized += () =>
        {
            m_graphicsDevice.MainSwapchain.Resize((uint)m_window.Width, (uint)m_window.Height);
            m_onResize?.Invoke(m_window.Width, m_window.Height);
        };
    }

    public void Run()
    {
        m_onLoad?.Invoke();
        m_onSetup?.Invoke();

        m_timer.Start();
        m_lastTime = 0.0;

        while (m_window.Exists)
        {
            double now = m_timer.Elapsed.TotalSeconds;
            float delta = (float)(now - m_lastTime);
            m_lastTime = now;

            m_onStep?.Invoke((float)now, delta);
            m_onEvent?.Invoke(m_window.PumpEvents());
            m_onDraw?.Invoke(delta);
        }

        m_onClose?.Invoke();
        m_graphicsDevice.Dispose();
    }

    public IGraphicsDevice GetGraphicsDevice() => m_graphicsDeviceVeldrid;
    public object GetWindowHolder() => m_window;
}
