using InnoEngine.Graphics.RenderPass;
using InnoInternal.Render.Impl;

namespace InnoEngine.Graphics;

/// <summary>
/// Controls and organizes all render passes.
/// </summary>
internal class RenderPipeline
{
    private readonly List<IRenderPass> m_passes = [];

    public void Register(IRenderPass pass)
    {
        m_passes.Add(pass);
        m_passes.Sort((a, b) => a.tag.CompareTo(b.tag));
    }

    public void Render(IRenderAPI api)
    {
        foreach (var pass in m_passes)
        {
            pass.Render(api);
        }
    }
}