using InnoEditor.Core;
using InnoInternal.ImGui.Impl;
using InnoInternal.Render.Impl;
using InnoInternal.Resource.Impl;

namespace InnoEditor.Panel;

public class SceneViewWindow : EditorWindow
{
    public override string Title => "Scene View";
    
    private readonly Action m_onSceneRender;
    
    private IRenderTarget? m_renderTarget;
    private ITexture2D? m_renderTexture;
    
    private int m_width = 0;
    private int m_height = 0;

    internal SceneViewWindow(Action onSceneRender)
    {
        m_onSceneRender = onSceneRender;
    }
    
    internal override void OnGUI(IImGuiContext context, IRenderAPI renderAPI)
    {
        // 获取可用区域大小
        var available = context.GetContentRegionAvail();
        int newWidth = (int)Math.Max(available.x, 1);
        int newHeight = (int)Math.Max(available.y, 1);

        // 分辨率变了就重建
        if (newWidth != m_width || newHeight != m_height || m_renderTarget == null)
        {
            m_width = newWidth;
            m_height = newHeight;

            m_renderTarget?.Dispose();
            m_renderTarget = renderAPI.command.CreateRenderTarget(m_width, m_height);
            m_renderTexture = m_renderTarget.GetColorTexture();
        }
        

        // 把场景渲染到这个 render target
        if (m_renderTarget != null)
        {
            renderAPI.command.SetRenderTarget(m_renderTarget);
            m_onSceneRender.Invoke();
            renderAPI.command.SetRenderTarget(null);
        }

        // 显示在窗口中
        if (m_renderTexture != null)
        {
            context.Image(m_renderTexture, m_width, m_height);
        }
    }
}