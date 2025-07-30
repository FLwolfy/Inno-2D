using InnoEditor.GUI;
using InnoEditor.GUI.InspectorGUI;
using InnoEditor.GUI.PropertyGUI;
using InnoEditor.Panel;
using InnoEngine.Core;
using InnoEngine.Utility;
using InnoInternal.ImGui.Bridge;
using InnoInternal.ImGui.Impl;
using InnoInternal.Render.Impl;

namespace InnoEditor.Core;

public abstract class EditorApp : EditorCore
{
    private readonly IImGuiRenderer m_imGuiRenderer = new ImGuiNETMonoGameRenderer();

    protected override void Setup()
    {
        // Renderer Setup
        m_imGuiRenderer.Initialize(GetWindowHolder(), GetWindowHolder());
        
        // GUI Setup
        EditorGUILayout.Initialize(m_imGuiRenderer.context);
        PropertyRendererRegistry.Initialize();
        InspectorEditorRegistry.Initialize();
        
        // MenuBar Setup
        // TODO: Setup MenuBar
        
        // Panel Setup
        EditorManager.RegisterWindow(new SceneViewPanel(TryRenderScene));
        EditorManager.RegisterWindow(new HierarchyPanel());
        EditorManager.RegisterWindow(new InspectorPanel());
        
        // Scene Setup
        SceneSetup();
    }

    protected override void OnEditorUpdate(float totalTime, float deltaTime)
    {
        // TODO
    }

    protected override void OnEditorGUI(float deltaTime)
    {
        m_imGuiRenderer.BeginLayout(deltaTime);
        EditorGUILayout.BeginFrame();
        EditorManager.DrawPanels(m_imGuiRenderer.context, (IRenderAPI) GetRenderAPI());
        EditorGUILayout.EndFrame();
        m_imGuiRenderer.EndLayout();
    }

    /// <summary>
    /// Sets up the initial scene.
    /// </summary>
    protected abstract void SceneSetup();
}
