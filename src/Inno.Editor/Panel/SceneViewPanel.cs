using Inno.Editor.Core;
using Inno.Editor.Gizmo;
using Inno.Editor.Utility;
using InnoBase;
using InnoBase.Event;
using InnoBase.Graphics;
using InnoBase.Math;
using InnoEngine.Graphics;
using InnoInternal.ImGui.Impl;
using InnoInternal.Render.Impl;

namespace Inno.Editor.Panel;

public class SceneViewPanel : EditorPanel
{
    public override string title => "Scene";

    private static readonly Color AXIS_COLOR = new Color(0.5f, 0.5f, 0.5f, 0.5f);
    private static readonly float AXIS_THICKNESS = 1.0f;
    private static readonly int AXIS_INTERVAL = 100;
    private static readonly float AXIS_INTERVAL_SCALE_RATE = 0.5f;
    private static readonly Input.MouseButton MOUSE_BUTTON_PAN = Input.MouseButton.Left;
    
    private readonly EditorCamera2D m_editorCamera2D = new EditorCamera2D();
    private readonly GridGizmo m_gridGizmo = new GridGizmo();
    
    private int m_width = 0;
    private int m_height = 0;
    
    internal SceneViewPanel()
    {
        m_gridGizmo.showCoords = true;
        m_gridGizmo.color = AXIS_COLOR;
        m_gridGizmo.lineThickness = AXIS_THICKNESS;
    }
    
    internal override void OnGUI(IImGuiContext imGuiContext, RenderContext renderContext)
    {
        // Ensure scene render target
        EnsureSceneRenderTarget(renderContext);
        
        // Check if region changed
        CheckRegionChange(imGuiContext, renderContext);

        // Handle editorCamera action
        HandlePanZoom(imGuiContext);

        // render and display scene on new render target
        RenderSceneToBuffer(renderContext);

        // display on scene view
        DrawScene(imGuiContext, renderContext);
        
        // Draw axis gizmo
        DrawAxisGizmo(imGuiContext);
    }
    
    private void EnsureSceneRenderTarget(RenderContext ctx)
    {
        if (ctx.targetPool.Get("scene") == null)
        {
            var renderTexDesc = new TextureDescription
            {
                format = PixelFormat.B8_G8_R8_A8_UNorm,
                usage = TextureUsage.RenderTarget | TextureUsage.Sampled,
                dimension = TextureDimension.Texture2D
            };
            
            var depthTexDesc = new TextureDescription
            {
                format = PixelFormat.D32_Float_S8_UInt,
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

    private void CheckRegionChange(IImGuiContext imGuiContext, RenderContext renderContext)
    {
        // Get Available region
        var available = imGuiContext.GetContentRegionAvail();
        int newWidth = (int)Math.Max(available.x, 1);
        int newHeight = (int)Math.Max(available.y, 1);
        
        // if region change, resize
        if (newWidth != m_width || newHeight != m_height)
        {
            m_width = newWidth;
            m_height = newHeight;
            
            m_editorCamera2D.SetViewportSize(newWidth, newHeight);
            renderContext.targetPool.Get("scene")?.Resize(newWidth, newHeight);
        }
    }

    private void RenderSceneToBuffer(RenderContext renderContext)
    {
        if (renderContext.targetPool.Get("scene") != null)
        {
            var flipYViewMatrix = m_editorCamera2D.viewMatrix;
            flipYViewMatrix.m42 *= -1;
            
            renderContext.renderer2D.BeginFrame(flipYViewMatrix * m_editorCamera2D.projectionMatrix, null, renderContext.targetPool.Get("scene"));
            renderContext.passController.RenderPasses(renderContext);
            renderContext.renderer2D.EndFrame();
        }
    }
    
    private void HandlePanZoom(IImGuiContext imGuiContext)
    {
        Vector2 panDelta = Vector2.ZERO;
        float zoomDelta = imGuiContext.GetMouseWheel();
        
        Vector2 windowPos = imGuiContext.GetWindowPos();
        Vector2 screenPos = imGuiContext.GetCursorStartPos();
        Vector2 mousePos = imGuiContext.GetMousePosition();
        Vector2 localMousePos = mousePos - screenPos - windowPos;

        bool isMouseInContent = localMousePos.y > 0 && imGuiContext.IsWindowHovered();
        bool isPanning = imGuiContext.IsMouseDown((int)MOUSE_BUTTON_PAN) || zoomDelta != 0.0f;
        if (isMouseInContent && isPanning)
        {
            if (imGuiContext.IsWindowFocused()) { panDelta = imGuiContext.GetMouseDelta(); }
            else { imGuiContext.SetWindowFocus(); }
        }

        if (imGuiContext.IsWindowFocused())
        {
            m_editorCamera2D.Update(panDelta, zoomDelta, localMousePos);
        }
    }

    private void DrawScene(IImGuiContext imGuiContext, RenderContext renderContext)
    {
        var targetTexture = renderContext.targetPool.Get("scene")?.GetColorAttachment(0);
        if (targetTexture != null)
        {
            imGuiContext.Image(targetTexture, m_width, m_height);
        }
    }
    
    private void DrawAxisGizmo(IImGuiContext imGuiContext)
    {
        Vector2 axisOriginWorld = Vector2.Transform(Vector2.ZERO, m_editorCamera2D.GetScreenToWorldMatrix());
        float spacing = Vector2.Transform(axisOriginWorld + new Vector2(AXIS_INTERVAL, 0), m_editorCamera2D.GetWorldToScreenMatrix()).x;
        int newAxisInterval = AXIS_INTERVAL;
        
        while (spacing < AXIS_INTERVAL * AXIS_INTERVAL_SCALE_RATE)
        {
            newAxisInterval = (int)(newAxisInterval / AXIS_INTERVAL_SCALE_RATE);
            spacing = Vector2.Transform(axisOriginWorld + new Vector2(newAxisInterval, 0), m_editorCamera2D.GetWorldToScreenMatrix()).x;
        }
        
        while (spacing > AXIS_INTERVAL / AXIS_INTERVAL_SCALE_RATE)
        {
            newAxisInterval = (int)(newAxisInterval * AXIS_INTERVAL_SCALE_RATE);
            spacing = Vector2.Transform(axisOriginWorld + new Vector2(newAxisInterval, 0), m_editorCamera2D.GetWorldToScreenMatrix()).x;
        }
        
        float offsetXWorld = (MathF.Floor(axisOriginWorld.x / newAxisInterval) + 1) * newAxisInterval;
        float offsetYWorld = (MathF.Floor(axisOriginWorld.y / newAxisInterval) + 1) * newAxisInterval;
        Vector2 offsetWorld = new Vector2(offsetXWorld, offsetYWorld);
        Vector2 offset = Vector2.Transform(offsetWorld, m_editorCamera2D.GetWorldToScreenMatrix());
        
        m_gridGizmo.startPos = imGuiContext.GetWindowPos() + imGuiContext.GetCursorStartPos();
        m_gridGizmo.size = new Vector2(m_width, m_height);
        m_gridGizmo.offset = offset;
        m_gridGizmo.spacing = spacing;
        m_gridGizmo.startCoords = offsetWorld;
        m_gridGizmo.coordsIncrement = new Vector2(newAxisInterval, newAxisInterval);
        
        m_gridGizmo.Draw(imGuiContext);
    }

}