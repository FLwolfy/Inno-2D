namespace InnoInternal.Window.Impl;

public interface IWindow
{
    bool exists { get; }
    
    int width { get; set; }
    int height { get; set; }
    bool resizable { get; set; }
    string title { get; set; }

    void Show();
    void Hide();
    void Close();

    event Action? OnResize;
    event Action? OnClose;

    void PumpEvents();
}
