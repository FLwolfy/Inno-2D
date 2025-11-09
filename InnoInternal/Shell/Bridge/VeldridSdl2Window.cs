using InnoBase;
using InnoInternal.Shell.Impl;
using Veldrid;
using Veldrid.Sdl2;

namespace InnoInternal.Shell.Bridge;

internal class VeldridSdl2Window : IWindow
{
    internal Sdl2Window inner { get; }
    internal InputSnapshot innerSnapshot { get; private set; }

    public event Action<Event> OnEvent = delegate { };

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
    }

    public void Show() => inner.Visible = true;
    public void Hide() => inner.Visible = false;
    public void Close() => inner.Close();

    public void PumpEvents()
    {
        innerSnapshot = inner.PumpEvents();
        
        VeldridSdl2EventDispatcher.Dispatch(innerSnapshot, e =>
        {
            OnEvent.Invoke(e);
        });
    }
}