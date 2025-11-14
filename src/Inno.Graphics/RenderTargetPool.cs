using Inno.Platform.Graphics;

namespace Inno.Graphics;

public class RenderTargetPool : IDisposable
{
    private readonly IGraphicsDevice m_device;
    private readonly Dictionary<string, IFrameBuffer> m_targets = new();
    
    public RenderTargetPool(IGraphicsDevice device)
    {
        m_device = device;
        m_targets["main"] = device.swapchainFrameBuffer;
    }

    public IFrameBuffer? Get(string name)
    {
        m_targets.TryGetValue(name, out var fb);
        return fb;
    }

    public IFrameBuffer GetMain()
    {
        return m_targets["main"];
    }

    public void Create(string name, FrameBufferDescription desc)
    {
        if (m_targets.TryGetValue(name, out _))
            throw new InvalidOperationException($"Already exists a framebuffer named {name}!");

        m_targets[name] = m_device.CreateFrameBuffer(desc);
    }

    public void Release(string name)
    {
        if (m_targets.TryGetValue(name, out var fb))
        {
            fb.Dispose();
            m_targets.Remove(name);
        }
    }

    public void Clear()
    {
        foreach (var fb in m_targets.Values)
        {
            fb.Dispose();
        }
        m_targets.Clear();
    }

    public void Dispose()
    {
        Clear();
    }
}
