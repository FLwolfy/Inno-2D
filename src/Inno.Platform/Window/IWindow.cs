using Inno.Core.Events;
using Inno.Platform.Window.Bridge;

namespace Inno.Platform.Window;

public enum WindowBackend
{
    Veldrid_Sdl2
}

public struct WindowInfo
{
    public string name;
    public int width;
    public int height;
}

public interface IWindow
{
    bool exists { get; }
    
    int width { get; set; }
    int height { get; set; }
    bool resizable { get; set; }
    bool decorated { get; set; }
    string title { get; set; }

    void Show();
    void Hide();
    void Close();

    void PumpEvents(EventDispatcher dispatcher);
}
