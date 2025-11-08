using InnoInternal.Render.Impl;
using InnoInternal.Shell.Bridge;

namespace InnoInternal.Shell.Impl;

public interface IGameShell
{
    public enum ShellType { Veldrid }

    static IGameShell CreateShell(ShellType type)
    {
        return type switch
        {
            ShellType.Veldrid => new VeldridShell(),
            _ => throw new NotSupportedException()
        };
    }

    // Window & App Controls
    void Run();
    void SetWindowSize(int width, int height);
    void SetWindowResizable(bool resizable);
    void SetOnResize(Action<int, int> onResize); // TODO: This should be replaced by OnEvent
    

    // Core lifecycle hooks
    void SetOnLoad(Action onLoad);
    void SetOnSetup(Action onSetup);
    void SetOnStep(Action<float, float> onStep);
    void SetOnEvent(Action<object> onEvent); // TODO: Define event type
    void SetOnDraw(Action<float> onDraw);
    void SetOnClose(Action onClose);

    // Provide handles for Editor
    IGraphicsDevice GetGraphicsDevice();
    object GetWindowHolder();
}