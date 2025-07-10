using InnoEngine.Core;
using InnoEngine.ECS;
using InnoEngine.ECS.Component;
using InnoEngine.Extension;
using Microsoft.Xna.Framework;

namespace Sandbox;

public static class Program
{
    /// <summary>
    /// Entry point of the test sandbox application.
    /// </summary>
    public static void Main()
    {
        using var game = new TestGame();
        game.Run();
    }
}

public class TestGame : GameShell
{
    public override void SetUp()
    {
        GameScene testScene = SceneManager.CreateScene("Test Scene");
        SceneManager.SetActiveScene(testScene);
        
        GameObject testObject = new GameObject("Test Object");
        testObject.transform.worldPosition = new Vector3(300, 300, 1);
        
        testObject.AddComponent<SpriteRenderer>();
        testObject.AddComponent<TestComponent>();
    }
}