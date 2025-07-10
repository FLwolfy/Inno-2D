namespace Engine.Graphics;

/// <summary>
/// Controls and organizes all render passes.
/// </summary>
public class RenderPipeline
{
    private readonly List<IRenderPass> m_passes = [];

    public void Register(IRenderPass pass)
    {
        m_passes.Add(pass);
        m_passes.Sort((a, b) => a.tag.CompareTo(b.tag));
    }

    public void Render(RenderContext context)
    {
        foreach (var pass in m_passes)
        {
            pass.Render(context);
        }
    }
}