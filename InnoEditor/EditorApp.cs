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
