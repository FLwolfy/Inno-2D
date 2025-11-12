using InnoEngine.Graphics.RenderPass.Pass;

namespace InnoEngine.Graphics.RenderPass;

/// <summary>
/// System to update and render all Renderer components in scene.
/// </summary>
public class RenderPassController
{
    private readonly List<IRenderPass> m_passes = [];

    public void LoadPasses()
    {
        RegisterPass(new ClearScreenPass());
        RegisterPass(new SpriteRenderPass());
        
        // TODO: Add more RenderPasses here
    }

    private void RegisterPass(IRenderPass renderPass)
    {
        m_passes.Add(renderPass);
        m_passes.Sort((a, b) => a.tag.CompareTo(b.tag));
    }

    public void RenderPasses(RenderContext ctx)
    {
        foreach (var pass in m_passes)
        {
            pass.Render(ctx);
        }
    }
}