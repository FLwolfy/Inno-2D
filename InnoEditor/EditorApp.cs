using InnoBase;
using InnoInternal.ImGui.Impl;
using InnoInternal.ImGui.Bridge;
using InnoEngine.Core;
using InnoEditor.Core;
using InnoEditor.Panel;
using InnoEngine.ECS;
using InnoEngine.ECS.Component;
using InnoInternal.Render.Impl;

namespace InnoEditor;

public class EditorApp : EditorCore
{
    private readonly IImGuiRenderer m_imGuiRenderer = new ImGuiNETMonoGameRenderer();

    protected override void Setup()
    {
        // Renderer Setup
        m_imGuiRenderer.Initialize(GetWindowHolder());
        
        // Panel Setup
        EditorManager.RegisterWindow(new SceneViewWindow(TryRenderScene));
        EditorManager.RegisterWindow(new InspectorWindow());
        
        // TODO: Setup Menu Bar
        
        // TODO: Remove this test scene
        GameScene testScene = SceneManager.CreateScene("Test Scene");
        SceneManager.SetActiveScene(testScene);
        
        GameObject testObject = new GameObject("Test Object");
        testObject.transform.worldPosition = new Vector3(300, 300, 3);
        
        testObject.AddComponent<SpriteRenderer>();
        testObject.transform.worldScale = new Vector3(100f, 200f, 1f);
        // tc.SetActive(false);
        
        GameObject testObject2 = new GameObject("Test Object2");
        testObject2.transform.worldPosition = new Vector3(300, 300, 5);
        testObject2.transform.worldScale = new Vector3(100f, 100f, 1f);
        testObject2.transform.SetParent(testObject.transform);
        
        SpriteRenderer sr2 = testObject2.AddComponent<SpriteRenderer>();
        sr2.color = Color.BLACK;
        sr2.layerDepth = 0;
        // sr2.SetActive(false);
    }

    protected override void OnEditorUpdate(float totalTime, float deltaTime)
    {
        // TODO
    }

    protected override void OnEditorGUI(float deltaTime)
    {
        m_imGuiRenderer.BeginLayout(deltaTime);
        EditorManager.DrawMenuBar(m_imGuiRenderer.context);
        EditorManager.DrawWindow(m_imGuiRenderer.context, (IRenderAPI) GetRenderAPI());
        m_imGuiRenderer.EndLayout();
    }
}
