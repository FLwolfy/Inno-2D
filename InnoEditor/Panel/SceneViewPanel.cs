using InnoBase;
using InnoEditor.Core;
using InnoEditor.Gizmo;
using InnoEditor.Utility;
using InnoInternal.ImGui.Impl;
using InnoInternal.Render.Impl;
using InnoInternal.Resource.Impl;

namespace InnoEditor.Panel;

public class SceneViewPanel : EditorPanel
{
    public override string title => "Scene View";

    private static readonly Color AXIS_COLOR = new Color(0.5f, 0.5f, 0.5f, 0.5f);
    private static readonly float AXIS_THICKNESS = 1.0f;
    private static readonly int AXIS_INTERVAL = 100;
    private static readonly float AXIS_INTERVAL_SCALE_RATE = 0.5f;
    
    private readonly Action<Matrix, Matrix> m_onSceneRender;
    private readonly EditorCamera2D m_editorCamera2D = new EditorCamera2D();
    private readonly GridGizmo m_gridGizmo = new GridGizmo();
    
    private IRenderTarget? m_renderTarget;
    private ITexture2D? m_renderTexture;
    
    private int m_width = 0;
    private int m_height = 0;
    
    internal SceneViewPanel(Action<Matrix, Matrix> onSceneRender)
    {
        m_onSceneRender = onSceneRender;
        
        m_gridGizmo.showCoords = true;
        m_gridGizmo.color = AXIS_COLOR;
        m_gridGizmo.lineThickness = AXIS_THICKNESS;
    }
    
    internal override void OnGUI(IImGuiContext context, IRenderAPI renderAPI)
    {
        // Get Available region
        var available = context.GetContentRegionAvail();
        int newWidth = (int)Math.Max(available.x, 1);
        int newHeight = (int)Math.Max(available.y, 1);
        
        // if region change, reset
        if (newWidth != m_width || newHeight != m_height || m_renderTarget == null)
        {
            m_width = newWidth;
            m_height = newHeight;
            m_editorCamera2D.SetViewportSize(m_width, m_height);

            m_renderTarget?.Dispose();
            m_renderTarget = renderAPI.command.CreateRenderTarget(m_width, m_height);
            m_renderTexture = m_renderTarget.GetColorTexture();
        }

        // Handle editorCamera action
        HandlePanZoom(context);

        // render scene on new render target
        if (m_renderTarget != null)
        {
            renderAPI.command.SetRenderTarget(m_renderTarget);
            renderAPI.command.SetViewport(new Rect(0, 0, m_width, m_height));
            
            m_onSceneRender.Invoke(m_editorCamera2D.viewMatrix, m_editorCamera2D.projectionMatrix);
            
            renderAPI.command.SetRenderTarget(null);
            renderAPI.command.SetViewport(null);
        }

        // display on scene view
        if (m_renderTexture != null)
        {
            context.Image(m_renderTexture, m_width, m_height);
        }
        
        // Draw axis gizmo
        DrawAxisGizmo(context);
    }
    
    private void HandlePanZoom(IImGuiContext context)
    {
        Vector2 panDelta = Vector2.ZERO;
        float zoomDelta = context.GetMouseWheel();

        if (context.IsMouseDown((int)Input.MouseButton.Middle))
        {
            if (context.IsWindowFocused()) { panDelta = context.GetMouseDelta(); }
            else { context.SetWindowFocus(); }
        }

        Vector2 windowPos = context.GetWindowPos();
        Vector2 screenPos = context.GetCursorStartPos();
        Vector2 mousePos = context.GetMousePosition();

        Vector2 localMousePos = mousePos - screenPos - windowPos;

        m_editorCamera2D.Update(panDelta, zoomDelta, localMousePos);
    }
    
    private void DrawAxisGizmo(IImGuiContext context)
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
        
        m_gridGizmo.startPos = context.GetWindowPos() + context.GetCursorStartPos();
        m_gridGizmo.size = new Vector2(m_width, m_height);
        m_gridGizmo.offset = offset;
        m_gridGizmo.spacing = spacing;
        m_gridGizmo.startCoords = offsetWorld;
        m_gridGizmo.coordsIncrement = new Vector2(newAxisInterval, newAxisInterval);
        
        m_gridGizmo.Draw(context);
    }

}