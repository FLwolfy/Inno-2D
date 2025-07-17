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
    
    void Run();
}