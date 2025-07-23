using InnoBase;
using InnoEditor.Core;
using InnoEngine.ECS;
using InnoEngine.ECS.Component;

namespace InnoEditor;

internal static class Program
{
    /// <summary>
    /// Entry point of the test sandbox application.
    /// </summary>
    public static void Main()
    {
        EditorApp editor = new TestEditorApp();
        editor.Run();
    }
}

internal class TestEditorApp : EditorApp
{
    protected override void SceneSetup()
    {
        GameScene testScene = SceneManager.CreateScene("Test Scene");
        SceneManager.SetActiveScene(testScene);
        
        // Object 1
        GameObject testObject = new GameObject("Test Object 1");
        testObject.transform.worldPosition = new Vector3(320, 180, 0);
        testObject.transform.worldScale = new Vector3(100f, 200f, 1f);
        testObject.AddComponent<SpriteRenderer>();
        
        // Object 2 - 5
        for (int i = 2; i <= 5; i++)
        {
            GameObject to = new GameObject("Test Object" + i);
            to.transform.worldPosition = new Vector3(150 * (i - 2), 0, 5);
            to.transform.worldScale = new Vector3(100f, 100f, 1f);
            SpriteRenderer sr = to.AddComponent<SpriteRenderer>();
            sr.color = Color.BLACK;
            
            to.transform.SetParent(testObject.transform);
        }
    }
}