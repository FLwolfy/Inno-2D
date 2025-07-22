using InnoBase;
using InnoEditor.Panel;
using InnoEngine.Core;
using InnoEngine.ECS;
using InnoEngine.ECS.Component;
using InnoInternal.ImGui.Bridge;
using InnoInternal.ImGui.Impl;
using InnoInternal.Render.Impl;

namespace InnoEditor.Core;

public class EditorApp : EditorCore
{
    private readonly IImGuiRenderer m_imGuiRenderer = new ImGuiNETMonoGameRenderer();

    protected override void Setup()
    {
        // Renderer Setup
        m_imGuiRenderer.Initialize(GetWindowHolder());
        
        // Panel Setup
        EditorManager.RegisterWindow(new SceneViewPanel(TryRenderScene));
        EditorManager.RegisterWindow(new HierarchyPanel());
        EditorManager.RegisterWindow(new InspectorPanel());
        
        // TODO: Setup Menu Bar
        
        // TODO: Remove Test Setup
        TestSceneSetup();
    }

    protected override void OnEditorUpdate(float totalTime, float deltaTime)
    {
        // TODO
    }

    protected override void OnEditorGUI(float deltaTime)
    {
        m_imGuiRenderer.BeginLayout(deltaTime);
        EditorManager.DrawPanels(m_imGuiRenderer.context, (IRenderAPI) GetRenderAPI());
        m_imGuiRenderer.EndLayout();
    }

    // TODO: Remove this
    private void TestSceneSetup()
    {
        GameScene testScene = SceneManager.CreateScene("Test Scene");
        SceneManager.SetActiveScene(testScene);
        
        // Object 1
        GameObject testObject = new GameObject("Test Object");
        testObject.transform.worldPosition = new Vector3(320, 180, 0);
        
        testObject.AddComponent<SpriteRenderer>();
        testObject.transform.worldScale = new Vector3(100f, 200f, 1f);
        
        // Object 2 - 5
        for (int i = 2; i <= 5; i++)
        {
            GameObject to = new GameObject("Test Object" + i);
            to.transform.worldPosition = new Vector3(150 * i, 0, 5);
            to.transform.worldScale = new Vector3(100f, 100f, 1f);
            SpriteRenderer sr = to.AddComponent<SpriteRenderer>();
            sr.color = Color.BLACK;
            
            to.transform.SetParent(testObject.transform);
        }
    }
}
