using InnoEditor.GUI;
using InnoEditor.GUI.InspectorGUI;
using InnoEditor.GUI.PropertyGUI;
using InnoEditor.Panel;
using InnoEngine.Core.Layer;
using InnoEngine.Graphics;
using InnoEngine.Utility;

using InnoInternal.Render.Impl;

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

    public override void OnRender(RenderContext ctx)
    {
        EnsureRenderTarget(ctx);
        
        ctx.imGuiRenderer.BeginLayout(Time.renderDeltaTime, null); // TODO: Change this
        EditorGUILayout.BeginFrame(ctx.imGuiRenderer.context);
        EditorManager.DrawPanels(ctx.imGuiRenderer.context, ctx);
        EditorGUILayout.EndFrame();
        ctx.imGuiRenderer.EndLayout();
    }

    // TODO: Make this customizable
    private void EnsureRenderTarget(RenderContext ctx)
    {
        if (ctx.targetPool.Get("scene") == null)
        {
            var renderTexDesc = new TextureDescription
            {
                format = PixelFormat.B8G8R8A8UNorm,
                usage = TextureUsage.RenderTarget | TextureUsage.Sampled,
                dimension = TextureDimension.Texture2D
            };
            
            var depthTexDesc = new TextureDescription
            {
                format = PixelFormat.D32FloatS8UInt,
                usage = TextureUsage.DepthStencil,
                dimension = TextureDimension.Texture2D
            };
            
            var renderTargetDesc = new FrameBufferDescription
            {
                depthAttachmentDescription = depthTexDesc,
                colorAttachmentDescriptions = [renderTexDesc]
            };
            
            ctx.targetPool.Create("scene", renderTargetDesc);
        }
    }
}