using InnoBase;
using InnoEditor.Core;
using InnoEditor.Gizmo;
using InnoEditor.Utility;
using InnoEngine.Graphics;
using InnoInternal.ImGui.Impl;
using InnoInternal.Render.Impl;

namespace InnoEditor.Panel;

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

    private ITexture? m_renderTexture;
    private ITexture? m_depthTexture;
    private IFrameBuffer? m_renderTarget;
    
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
        // Check if region changed
        CheckRegionChange(imGuiContext, renderContext);

        // Handle editorCamera action
        HandlePanZoom(imGuiContext);

        // render and display scene on new render target
        RenderSceneToBuffer(renderContext);

        // display on scene view
        DrawScene(imGuiContext);
        
        // Draw axis gizmo
        DrawAxisGizmo(imGuiContext);
    }

    private void CheckRegionChange(IImGuiContext imGuiContext, RenderContext renderContext)
    {
        // Get Available region
        var available = imGuiContext.GetContentRegionAvail();
        int newWidth = (int)Math.Max(available.x, 1);
        int newHeight = (int)Math.Max(available.y, 1);
        
        // if region change, reset
        if (newWidth != m_width || newHeight != m_height || m_renderTarget == null)
        {
            RecreateRenderTarget(renderContext, newWidth, newHeight);
        }
    }

    private void RecreateRenderTarget(RenderContext renderContext, int width, int height)
    {
        // Update size
        m_width = width;
        m_height = height;
        m_editorCamera2D.SetViewportSize(m_width, m_height);

        // Recreate render target
        m_depthTexture?.Dispose();
        m_renderTexture?.Dispose();
        m_renderTarget?.Dispose();

        var renderTexDesc = new TextureDescription
        {
            width = m_width,
            height = m_height,
            format = PixelFormat.B8G8R8A8UNorm,
            usage = TextureUsage.RenderTarget | TextureUsage.Sampled,
            dimension = TextureDimension.Texture2D
        };
            
        var depthTexDesc = new TextureDescription
        {
            width = m_width,
            height = m_height,
            format = PixelFormat.D32FloatS8UInt,
            usage = TextureUsage.DepthStencil,
            dimension = TextureDimension.Texture2D
        };
            
        m_depthTexture = renderContext.graphicsDevice.CreateTexture(depthTexDesc);
        m_renderTexture = renderContext.graphicsDevice.CreateTexture(renderTexDesc);
            
        var renderTargetDesc = new FrameBufferDescription
        {
            depthAttachment = m_depthTexture,
            colorAttachments = [m_renderTexture]
        };
            
        m_renderTarget = renderContext.graphicsDevice.CreateFrameBuffer(renderTargetDesc);
    }

    private void RenderSceneToBuffer(RenderContext renderContext)
    {
        if (m_renderTarget != null)
        {
            var flipYViewMatrix = m_editorCamera2D.viewMatrix;
            flipYViewMatrix.m42 *= -1;
            
            renderContext.renderer.BeginFrame(flipYViewMatrix * m_editorCamera2D.projectionMatrix, null, m_renderTarget);
            renderContext.passController.RenderPasses(renderContext);
            renderContext.renderer.EndFrame();
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

    private void DrawScene(IImGuiContext imGuiContext)
    {
        if (m_renderTexture != null)
        {
            imGuiContext.Image(m_renderTexture, m_width, m_height);
        }
    }
    
    private void DrawAxisGizmo(IImGuiContext imGuiContext)
    {
        Vector2 axisOriginWorld = Vector2.Transform(Vector2.ZERO, m_editorCamera2D.GetScreenToWorldMatrix());
        float spacing = Vector2.Transform(axisOriginWorld + new Vector2(AXIS_INTERVAL, 0), m_editorCamera2D.GetWorldToScreenMatrix()).x;;
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