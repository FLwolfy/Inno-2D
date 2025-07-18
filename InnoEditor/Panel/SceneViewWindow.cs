using InnoBase;
using InnoEditor.Core;
using InnoInternal.ImGui.Impl;
using InnoInternal.Render.Impl;
using InnoInternal.Resource.Impl;

namespace InnoEditor.Panel;

public class SceneViewWindow : EditorWindow
{
    public override string Title => "Scene View";
    
    private readonly Action<Matrix> m_onSceneRender;
    
    private IRenderTarget? m_renderTarget;
    private ITexture2D? m_renderTexture;
    
    private int m_width = 0;
    private int m_height = 0;

    internal SceneViewWindow(Action<Matrix> onSceneRender)
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

            m_renderTarget?.Dispose();
            m_renderTarget = renderAPI.command.CreateRenderTarget(m_width, m_height);
            m_renderTexture = m_renderTarget.GetColorTexture();
        }
        

        // render scene on new render target
        if (m_renderTarget != null)
        {
            renderAPI.command.SetRenderTarget(m_renderTarget);
            renderAPI.command.SetViewport(new Rect(0, 0, m_width, m_height));
            
            m_onSceneRender.Invoke();
            
            renderAPI.command.SetRenderTarget(null);
            renderAPI.command.SetViewport(null);
        }

        // display on scene view
        if (m_renderTexture != null)
        {
            context.Image(m_renderTexture, m_width, m_height);
        }
    }
}