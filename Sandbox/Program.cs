using InnoEngine.Core;
using InnoEngine.ECS;
using InnoEngine.ECS.Component;
using InnoEngine.Internal.Base;


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

public class TestGame : SandboxShell
{
    public override void SetUp()
    {
        GameScene testScene = SceneManager.CreateScene("Test Scene");
        SceneManager.SetActiveScene(testScene);
        
        GameObject testObject = new GameObject("Test Object");
        testObject.transform.worldPosition = new Vector3(300, 300, 3);
        
        testObject.AddComponent<SpriteRenderer>();
        TestComponent tc = testObject.AddComponent<TestComponent>();
        testObject.transform.worldScale = new Vector3(0.5f, 2f, 1.0f);
        // tc.SetActive(false);
        
        GameObject testObject2 = new GameObject("Test Object2");
        testObject2.transform.worldPosition = new Vector3(300, 300, 2);
        
        SpriteRenderer sr2 = testObject2.AddComponent<SpriteRenderer>();
        sr2.color = Color.BLACK;
        sr2.layerDepth = 0;
        // sr2.SetActive(false);
    }
}