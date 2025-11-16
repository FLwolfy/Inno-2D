using ImGuiNET;
using Inno.Core.Layers;
using Inno.Editor.GUI;
using Inno.Editor.GUI.InspectorGUI;
using Inno.Editor.GUI.PropertyGUI;
using Inno.Editor.Panel;

namespace Inno.Editor.Core;

public class EditorLayer: Layer
{
    public EditorLayer() : base("EditorLayer") {}
    
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
        // DockSpace
        ImGui.DockSpaceOverViewport(ImGui.GetMainViewport().ID);
        
        // Layout GUI
        EditorGUILayout.BeginFrame();
        EditorManager.DrawPanels();
        EditorGUILayout.EndFrame();
    }
}