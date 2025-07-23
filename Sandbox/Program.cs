using InnoBase;
using InnoEngine.Core;
using InnoEngine.ECS;
using InnoEngine.ECS.Component;

namespace Sandbox;

internal static class Program
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

internal class TestGame : EngineCore
{
    protected override void Setup()
    {
        GameScene testScene = SceneManager.CreateScene("Test Scene");
        SceneManager.SetActiveScene(testScene);
        
        // Camera
        GameObject camera = new GameObject("Camera");
        OrthographicCamera cameraComponent = camera.AddComponent<OrthographicCamera>();
        // camera.AddComponent<TestComponent>();
        camera.transform.worldPosition = new Vector3(0, 180, 0);
        cameraComponent.isMainCamera = true;
        cameraComponent.size = 720;
        
        // Object 1
        GameObject testObject = new GameObject("Test Object");
        testObject.transform.worldPosition = new Vector3(640, 360, 0);
        
        testObject.AddComponent<SpriteRenderer>();
        testObject.transform.worldScale = new Vector3(100f, 200f, 1f);
        
        // Object 2
        GameObject testObject2 = new GameObject("Test Object2");
        testObject2.transform.worldPosition = new Vector3(0, 0, 5);
        testObject2.transform.worldScale = new Vector3(100f, 100f, 1f);
        testObject2.transform.SetParent(testObject.transform);
        
        SpriteRenderer sr2 = testObject2.AddComponent<SpriteRenderer>();
        testObject2.AddComponent<TestComponent>();
        sr2.color = Color.BLACK;
        // sr2.SetActive(false);
    }
}