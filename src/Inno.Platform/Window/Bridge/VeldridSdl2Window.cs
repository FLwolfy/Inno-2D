using Inno.Core.Events;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;

namespace Inno.Platform.Window.Bridge;

internal class VeldridSdl2Window : IWindow
{
    internal Sdl2Window inner { get; }
    internal InputSnapshot inputSnapshot { get; private set; }

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
    public bool decorated
    {
        get => inner.BorderVisible;
        set => inner.BorderVisible = value;
    }
    public string title
    {
        get => inner.Title;
        set => inner.Title = value;
    }

    public VeldridSdl2Window(WindowInfo info)
    {
        inner = VeldridStartup.CreateWindow(new()
        {
            WindowTitle = info.name,
            WindowWidth = info.width,
            WindowHeight = info.height,
            WindowInitialState = WindowState.Normal
        });
    }

    public void Show() => inner.Visible = true;
    public void Hide() => inner.Visible = false;
    public void Close() => inner.Close();
    public void Resize(int w, int h) => Sdl2Native.SDL_SetWindowSize(inner.Handle, w, h);

    public void PumpEvents(EventDispatcher dispatcher)
    {
        inputSnapshot = inner.PumpEvents();
        VeldridSdl2EventAdapter.Adapt(inputSnapshot, dispatcher.PushEvent);
    }
}