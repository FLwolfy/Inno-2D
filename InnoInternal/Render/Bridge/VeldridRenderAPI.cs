using InnoInternal.Render.Impl;
using InnoInternal.Resource.Bridge;
using InnoInternal.Resource.Impl;

using Veldrid;

namespace InnoInternal.Render.Bridge;

internal class VeldridRenderAPI : IRenderAPI
{
    public IRenderer2D renderer2D { get; } = new VeldridRenderer2D();
    public IRenderContext renderContext { get; } = new VeldridRenderContext();
    public IAssetLoader renderAssetLoader { get; } = new VeldridAssetLoader();

    public void Initialize(object graphicDevice)
    {
        if (graphicDevice is not GraphicsDevice device)
            throw new ArgumentException("Invalid data type. Expected GraphicsDevice.", nameof(graphicDevice));
        
        var command = new VeldridRenderCommand(device);
        
        renderer2D.Initialize(command);
        renderContext.Initialize(command);
        renderAssetLoader.Initialize(command);
    }
}