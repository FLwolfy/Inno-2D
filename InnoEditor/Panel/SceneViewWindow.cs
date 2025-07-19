using InnoBase;
using InnoEditor.Core;
using InnoInternal.ImGui.Impl;
using InnoInternal.Render.Impl;
using InnoInternal.Resource.Impl;

namespace InnoEditor.Panel;

public class SceneViewWindow : EditorWindow
{
    public override string Title => "Scene View";

    private static readonly Color AXIS_COLOR = new Color(0.5f, 0.5f, 0.5f, 0.5f);
    private static readonly float AXIS_THICKNESS = 1.0f;
    private static readonly int AXIS_INTERVAL = 100;
    
    private readonly Action<Matrix, Matrix> m_onSceneRender;
    
    private IRenderTarget? m_renderTarget;
    private ITexture2D? m_renderTexture;
    
    private int m_width = 0;
    private int m_height = 0;
    
    private EditorCamera2D m_editorCamera2D = new EditorCamera2D();

    internal SceneViewWindow(Action<Matrix, Matrix> onSceneRender)
    {
        m_onSceneRender = onSceneRender;
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
        float zoomDelta = context.GetMouseWheel();;

        if (context.IsMouseDown((int)Input.MouseButton.Middle))
        {
            panDelta = context.GetMouseDelta();
        }

        Vector2 windowPos = context.GetWindowPosition();
        Vector2 screenPos = context.GetCursorStartPos();
        Vector2 mousePos = context.GetMousePosition();

        Vector2 localMousePos = mousePos - screenPos - windowPos;

        m_editorCamera2D.Update(panDelta, zoomDelta, localMousePos);
    }
    
    private void DrawAxisGizmo(IImGuiContext context)
    {
        const float axisInterval = 100f;
        
        Vector2 windowPos = context.GetWindowPosition();
        Vector2 screenPos = context.GetCursorStartPos();

        Vector2 axisOrigin = windowPos + screenPos; // 图像左上角
        Vector2 axisOriginWorld = Vector2.Transform(axisOrigin, m_editorCamera2D.GetScreenToWorldMatrix());

        float boundBottom = axisOrigin.y + m_height;
        float boundBottomWorld = Vector2.Transform(axisOrigin + new Vector2(0, m_height), m_editorCamera2D.GetScreenToWorldMatrix()).y;

        float boundRight = axisOrigin.x + m_width;
        float boundRightWorld = Vector2.Transform(axisOrigin + new Vector2(m_width, 0), m_editorCamera2D.GetScreenToWorldMatrix()).x;

        for (float i = axisOriginWorld.x; i < boundRightWorld; i += axisInterval)
        {
            float currentX = Vector2.Transform(new Vector2(i, 0), m_editorCamera2D.GetWorldToScreenMatrix()).x;

            context.DrawLine(
                new Vector2(currentX, axisOrigin.y),  // 从底部
                new Vector2(currentX, boundBottom),      // 到顶部
                AXIS_COLOR,
                AXIS_THICKNESS);
        }
        
        for (float j = axisOriginWorld.y; j < boundBottomWorld; j += axisInterval)
        {
            float currentY = Vector2.Transform(new Vector2(0, j), m_editorCamera2D.GetWorldToScreenMatrix()).y;

            context.DrawLine(
                new Vector2(axisOrigin.x, currentY),  // 从左侧
                new Vector2(boundRight, currentY),      // 到右侧
                AXIS_COLOR,
                AXIS_THICKNESS);
        }
    }

}