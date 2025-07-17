using InnoBase;
using InnoEngine.Core;
using InnoEngine.ECS;
using InnoEngine.ECS.Component;


namespace Sandbox;

public static class Program
{
    /// <summary>
    /// Entry point of the test sandbox application.
    /// </summary>
    public static void Main()
    {
        var game = new TestGame();
        game.Run();
    }
}

public class TestGame : EngineCore
{
    protected override void Setup()
    {
        GameScene testScene = SceneManager.CreateScene("Test Scene");
        SceneManager.SetActiveScene(testScene);
        
        // Camera
        GameObject camera = new GameObject("Camera");
        OrthographicCamera cameraComponent = camera.AddComponent<OrthographicCamera>();
        cameraComponent.isMainCamera = true;
        cameraComponent.size = 300;
        
        // Object 1
        GameObject testObject = new GameObject("Test Object");
        testObject.transform.worldPosition = new Vector3(300, 300, 3);
        
        testObject.AddComponent<SpriteRenderer>();
        TestComponent tc = testObject.AddComponent<TestComponent>();
        testObject.transform.worldScale = new Vector3(100f, 200f, 1f);
        // tc.SetActive(false);
        
        // Object 2
        GameObject testObject2 = new GameObject("Test Object2");
        testObject2.transform.worldPosition = new Vector3(300, 300, 5);
        testObject2.transform.worldScale = new Vector3(100f, 100f, 1f);
        testObject2.transform.SetParent(testObject.transform);
        
        SpriteRenderer sr2 = testObject2.AddComponent<SpriteRenderer>();
        sr2.color = Color.BLACK;
        sr2.layerDepth = 0;
        // sr2.SetActive(false);
    }
}