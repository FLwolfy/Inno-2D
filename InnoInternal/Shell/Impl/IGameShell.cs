namespace InnoInternal.Shell.Impl;

internal interface IGameShell
{
    void SetOnLoad(Action callback);
    void SetOnSetup(Action callback);
    void SetOnStep(Action<float, float> callback); // totalTime, deltaTime
    void SetOnDraw(Action<float> callback);        // deltaTime
    void SetOnClose(Action callback);
    void SetWindowResizable(bool enable);
    void SetOnWindowSizeChanged(Action<int, int> callback);

    // TODO: Change the following to specific types
    object GetGraphicsDevice();
    object GetWindowHolder();
    
    void Run();
}