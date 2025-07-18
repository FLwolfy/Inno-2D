using InnoBase;
using InnoEditor.Core;
using InnoInternal.ImGui.Impl;
using InnoInternal.Render.Impl;
using InnoInternal.Resource.Impl;

namespace InnoEditor.Panel;

public class SceneViewWindow : EditorWindow
{
    public override string Title => "Scene View";
    
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
            
            m_onSceneRender.Invoke(m_editorCamera2D.GetViewMatrix(), m_editorCamera2D.GetProjectionMatrix());
            
            renderAPI.command.SetRenderTarget(null);
            renderAPI.command.SetViewport(null);
        }

        // display on scene view
        if (m_renderTexture != null)
        {
            context.Image(m_renderTexture, m_width, m_height);
        }
    }
    
    private void HandlePanZoom(IImGuiContext context)
    {
        Vector2 panDelta = Vector2.ZERO;
        float zoomDelta = context.GetMouseWheel();;

        if (context.IsMouseDown((int)Input.MouseButton.Middle))
        {
            panDelta = context.GetMouseDelta();
        }
        
        Vector2 mousePos = context.GetMousePosition();
        Vector2 screenPos = context.GetCursorScreenPos();
        Vector2 localMousePos = mousePos - screenPos;

        m_editorCamera2D.Update(panDelta, zoomDelta, localMousePos);
    }
}