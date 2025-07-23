using InnoEditor.Panel;
using InnoEngine.Core;
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
        m_imGuiRenderer.Initialize(GetWindowHolder());
        
        // Panel Setup
        EditorManager.RegisterWindow(new SceneViewPanel(TryRenderScene));
        EditorManager.RegisterWindow(new HierarchyPanel());
        EditorManager.RegisterWindow(new InspectorPanel());
        
        // TODO: Setup Menu Bar
        
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
        EditorManager.DrawPanels(m_imGuiRenderer.context, (IRenderAPI) GetRenderAPI());
        m_imGuiRenderer.EndLayout();
    }

    /// <summary>
    /// Sets up the initial scene.
    /// </summary>
    protected abstract void SceneSetup();
}
