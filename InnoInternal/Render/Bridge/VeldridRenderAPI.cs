using InnoInternal.Render.Impl;

using Veldrid;

namespace InnoInternal.Render.Bridge;

internal class VeldridRenderAPI : IRenderAPI
{
    private GraphicsDevice m_graphicsDevice = null!;
    
    public IRenderContext context { get; private set; } = null!;
    public IRenderCommand command { get; private set; } = null!;
    public IRenderer2D renderer2D { get; private set; } = null!;

    public void Initialize(object graphicDevice)
    {
        if (graphicDevice is not GraphicsDevice device)
        {
            throw new ArgumentException("Invalid data type. Expected GraphicsDevice.", nameof(graphicDevice));
        }
        
        m_graphicsDevice = device;
        
        command = new VeldridRenderCommand(m_graphicsDevice);
        context = new VeldridRenderContext(m_graphicsDevice, (VeldridRenderCommand)command);
        renderer2D = new MonoGameRenderer2D(context);
    }
}