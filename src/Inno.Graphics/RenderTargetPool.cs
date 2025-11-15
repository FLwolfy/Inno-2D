using Inno.Platform.Graphics;

namespace Inno.Graphics;

public static class RenderTargetPool
{
    private static readonly Dictionary<string, IFrameBuffer> TARGETS = new();
    private static IGraphicsDevice m_device = null!;

    public static void Initialize(IGraphicsDevice device)
    {
        m_device = device;
        TARGETS["main"] = device.swapchainFrameBuffer;
    }

    public static IFrameBuffer? Get(string name) => TARGETS.GetValueOrDefault(name);
    public static IFrameBuffer GetMain() => TARGETS["main"];

    public static void Create(string name, FrameBufferDescription desc)
    {
        if (TARGETS.ContainsKey(name))
            throw new InvalidOperationException($"Already exists a framebuffer named {name}!");
        TARGETS[name] = m_device.CreateFrameBuffer(desc);
    }

    public static void Release(string name)
    {
        if (TARGETS.TryGetValue(name, out var fb))
        {
            fb.Dispose();
            TARGETS.Remove(name);
        }
    }

    public static void Clear()
    {
        foreach (var fb in TARGETS.Values) fb.Dispose();
        TARGETS.Clear();
    }
}