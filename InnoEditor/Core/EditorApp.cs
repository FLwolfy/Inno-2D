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

    private void TestSceneSetup()
    {
        GameScene testScene = SceneManager.CreateScene("Test Scene");
        SceneManager.SetActiveScene(testScene);
        
        // Object 1
        GameObject testObject = new GameObject("Test Object");
        testObject.transform.worldPosition = new Vector3(320, 180, 0);
        
        testObject.AddComponent<SpriteRenderer>();
        testObject.transform.worldScale = new Vector3(100f, 200f, 1f);
        
        // Object 2
        GameObject testObject2 = new GameObject("Test Object2");
        testObject2.transform.worldPosition = new Vector3(0, 0, 5);
        testObject2.transform.worldScale = new Vector3(100f, 100f, 1f);
        testObject2.transform.SetParent(testObject.transform);
        
        SpriteRenderer sr2 = testObject2.AddComponent<SpriteRenderer>();
        sr2.color = Color.BLACK;
        // sr2.SetActive(false);
    }
}
