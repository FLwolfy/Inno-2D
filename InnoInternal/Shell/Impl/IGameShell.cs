namespace InnoInternal.Shell.Impl;

internal interface IGameShell
{
    void SetOnLoad(Action callback);
    void SetOnSetUp(Action callback);
    void SetOnStep(Action<float, float> callback); // totalTime, deltaTime
    void SetOnDraw(Action<float> callback);        // deltaTime
    void SetOnClose(Action callback);

    object GetShellData();

    void Run();
}