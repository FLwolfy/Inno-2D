using InnoInternal.Window.Impl;

using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;

namespace InnoInternal.Window.Bridge;

internal class VeldridSdl2Window : IWindow
{
    internal Sdl2Window inner { get; }

    public event Action? OnResize;
    public event Action? OnClose;

    public bool exists => inner.Exists;
    public int width
    {
        get => inner.Width;
        set => inner.Width = value;
    }
    public int height
    {
        get => inner.Height;
        set => inner.Height = value;
    }
    public bool resizable
    {
        get => inner.Resizable;
        set => inner.Resizable = value;
    }

    public string title
    {
        get => inner.Title;
        set => inner.Title = value;
    }

    public VeldridSdl2Window(Sdl2Window sdl2Window)
    {
        inner = sdl2Window;
        inner.Resized += () => OnResize?.Invoke();
        inner.Closed += () => OnClose?.Invoke();
    }

    public void Show() => inner.Visible = true;
    public void Hide() => inner.Visible = false;
    public void Close() => inner.Close();

    public void PumpEvents() => inner.PumpEvents();
}