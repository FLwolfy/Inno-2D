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
    private Action<float>? m_onDraw;
    private Action? m_onClose;
    private Action<int, int>? m_onResize;

    private readonly Sdl2Window m_window;
    private readonly GraphicsDevice m_graphicsDevice;
    private readonly VeldridGraphicsDevice m_graphicsDeviceVeldrid;
    private readonly Stopwatch m_timer = new();
    
    private double m_lastTime;

    private int m_width = 1280, m_height = 720;
    private bool m_resizable = true;

    public void SetWindowSize(int width, int height) => (m_width, m_height) = (width, height);
    public void SetWindowResizable(bool resizable) => m_resizable = resizable;

    public void SetOnLoad(Action onLoad) => m_onLoad = onLoad;
    public void SetOnSetup(Action onSetup) => m_onSetup = onSetup;
    public void SetOnStep(Action<float, float> onStep) => m_onStep = onStep;
    public void SetOnDraw(Action<float> onDraw) => m_onDraw = onDraw;
    public void SetOnClose(Action onClose) => m_onClose = onClose;
    public void SetOnResize(Action<int, int> onResize) => m_onResize = onResize;

    public VeldridShell()
    {
        var windowCi = new WindowCreateInfo
        {
            WindowWidth = m_width,
            WindowHeight = m_height,
            WindowTitle = "InnoEngine",
            WindowInitialState = m_resizable ? WindowState.Normal : WindowState.BorderlessFullScreen
        };
        
        VeldridStartup.CreateWindowAndGraphicsDevice(windowCi, out m_window, out m_graphicsDevice);
        m_graphicsDeviceVeldrid = new VeldridGraphicsDevice(m_graphicsDevice);
        
        m_window.Resized += () =>
        {
            m_width = m_window.Width;
            m_height = m_window.Height;
            m_graphicsDevice.MainSwapchain.Resize((uint)m_width, (uint)m_height);
            m_onResize?.Invoke(m_width, m_height);
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
            m_onDraw?.Invoke(delta);
        }

        m_onClose?.Invoke();
        m_graphicsDevice.Dispose();
    }

    public IGraphicsDevice GetGraphicsDevice() => m_graphicsDeviceVeldrid;
    public object GetWindowHolder() => m_window;
}
