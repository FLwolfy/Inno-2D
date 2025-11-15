using Inno.Core.Layers;
using Inno.Core.Utility;
using Inno.Editor.GUI;
using Inno.Editor.GUI.InspectorGUI;
using Inno.Editor.GUI.PropertyGUI;
using Inno.Editor.Panel;
using Inno.Graphics;
using Inno.Platform.ImGui;

namespace Inno.Editor.Core;

public class EditorLayer: Layer
{
    private readonly RenderContext m_renderContext;
    
    public EditorLayer() : base("EditorLayer")
    {
        m_renderContext = new RenderContext();
        
    }
    
    public override void OnAttach()
    {
        // GUI Initialization
        PropertyRendererRegistry.Initialize();
        InspectorEditorRegistry.Initialize();
        
        // MenuBar Setup
        // TODO: Setup MenuBar
        
        // Panel Registration
        EditorManager.RegisterPanel(new SceneViewPanel());
        EditorManager.RegisterPanel(new HierarchyPanel());
        EditorManager.RegisterPanel(new InspectorPanel());
    }

    public override void OnImGui()
    {
        // EditorGUILayout.BeginFrame();
        // EditorManager.DrawPanels();
        // EditorGUILayout.EndFrame();
    }
}