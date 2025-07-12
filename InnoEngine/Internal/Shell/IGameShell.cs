namespace InnoEngine.Internal.Shell;

public interface IGameShell
{
    void Load();
    void SetUp();
    void Step(float totalTime, float deltaTime);
    void Draw(float deltaTime);
}