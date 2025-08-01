using InnoInternal.Render.Impl;
using InnoInternal.Shell.Bridge;

namespace InnoInternal.Shell.Impl;

internal interface IGameShell
{
    // Callbacks
    void SetOnLoad(Action callback);
    void SetOnSetup(Action callback);
    void SetOnStep(Action<float, float> callback); // totalTime, deltaTime
    void SetOnDraw(Action<float> callback);        // deltaTime
    void SetOnClose(Action callback);
    
    // Window Events
    void SetWindowResizable(bool enable);
    void SetWindowSize(int width, int height);
    void SetOnWindowSizeChanged(Action<int, int> callback);

    // Render Objects
    // TODO: Change the following to specific types
    object GetGraphicsDevice();
    object GetWindowHolder();
    IRenderAPI GetRenderAPI();
    
    // Start
    void Run();
    
    // Create Shell
    enum ShellType { MonoGame }
    
    static IGameShell CreateShell(ShellType shellType)
    {
        return shellType switch
        {
            ShellType.MonoGame => new MonoGameShell(),
            
            // Default case to handle unsupported shell types
            _ => throw new NotSupportedException($"Shell type {shellType} is not supported.")
        };
    }
    
}