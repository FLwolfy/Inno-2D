using Inno.Graphics.Passes;
using Inno.Platform.Graphics;

namespace Inno.Graphics;

public class RenderContext : IDisposable
{
    private static IGraphicsDevice m_device = null!;
    
    public Renderer2D renderer2D { get; }
    public RenderPassStack passStack { get; }
    
    public RenderContext()
    {
        renderer2D = new Renderer2D(m_device);
        passStack = new RenderPassStack();
    }

    public static void Initialize(IGraphicsDevice device)
    {
        m_device = device;
    }

    public void Dispose()
    {
        renderer2D.Dispose();
    }
}