namespace Inno.Graphics.Passes;

/// <summary>
/// System to update and render all Renderer components in scene.
/// </summary>
public class RenderPassStack
{
    private readonly Dictionary<RenderPassTag, List<RenderPass>> m_passes = new();

    public void RegisterPass(RenderPass renderPass)
    {
        if (!m_passes.TryGetValue(renderPass.orderTag, out var list))
        {
            list = new List<RenderPass>();
            m_passes[renderPass.orderTag] = list;
        }
        
        list.Add(renderPass);
    }

    public void RenderPasses(RenderContext ctx)
    {
        foreach (RenderPassTag tag in Enum.GetValues(typeof(RenderPassTag)))
        {
            if (m_passes.TryGetValue(tag, out var list))
            {
                foreach (var pass in list)
                {
                    pass.Render(ctx);
                }
            }
        }
    }
}