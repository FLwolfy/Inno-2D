using InnoBase;
using InnoEditor.GUI;
using InnoEditor.GUI.InspectorGUI;
using InnoEditor.GUI.PropertyGUI;
using InnoEditor.Panel;
using InnoEngine.Core.Layer;
using InnoEngine.Graphics;
using InnoEngine.Utility;

namespace InnoEditor.Core;

public class EditorLayer() : Layer("EditorLayer")
{
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

    public override void OnEvent(Event e)
    {
        Console.WriteLine(e.type);
    }

    public override void OnUpdate()
    {
    }

    public override void OnRender(RenderContext ctx)
    {
        ctx.imGuiRenderer.BeginLayout(Time.renderDeltaTime, null);
        EditorGUILayout.BeginFrame(ctx.imGuiRenderer.context);
        EditorManager.DrawPanels(ctx.imGuiRenderer.context, ctx);
        EditorGUILayout.EndFrame();
        ctx.imGuiRenderer.EndLayout();
    }
}