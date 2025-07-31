using InnoEngine.Graphics.RenderPass;
using InnoInternal.Render.Impl;

namespace InnoEngine.Graphics;

/// <summary>
/// System to update and render all Renderer components in scene.
/// </summary>
internal class RenderSystem
{
    private RenderPipeline m_renderPipeline = null!;
    private IRenderAPI m_renderAPI = null!;

    public void Begin()
    {
        m_renderAPI.renderer2D.BeginScene();
    }
    
    public void End()
    {
        m_renderAPI.renderer2D.BeginScene();
    }
    
    public void Initialize(IRenderAPI api)
    {
        m_renderAPI = api;
        m_renderPipeline = new RenderPipeline();
    }

    public void LoadPasses()
    {
        // TODO: Add more RenderPasses here
        m_renderPipeline.Register(new ClearScreenPass());
        m_renderPipeline.Register(new SpriteRenderPass());
    }

    public void RenderPasses()
    {
        m_renderPipeline.Render(m_renderAPI);
    }
}