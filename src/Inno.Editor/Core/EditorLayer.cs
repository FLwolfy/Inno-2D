using Inno.Editor.GUI;
using Inno.Editor.GUI.InspectorGUI;
using Inno.Editor.GUI.PropertyGUI;
using Inno.Editor.Panel;
using InnoEngine.Core.Layer;
using InnoEngine.Graphics;
using InnoEngine.Utility;

using InnoInternal.Render.Impl;

namespace Inno.Editor.Core;

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

    public override void OnRender(RenderContext ctx)
    {
        ctx.imGuiRenderer.BeginLayout(Time.renderDeltaTime, ctx.targetPool.GetMain()); // TODO: Use Renderer2D Blit
        EditorGUILayout.BeginFrame(ctx.imGuiRenderer.context);
        EditorManager.DrawPanels(ctx.imGuiRenderer.context, ctx);
        EditorGUILayout.EndFrame();
        ctx.imGuiRenderer.EndLayout();
    }
}