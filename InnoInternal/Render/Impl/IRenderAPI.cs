using InnoInternal.Resource.Impl;

namespace InnoInternal.Render.Impl;

internal interface IRenderAPI
{
    IRenderer2D renderer2D { get; }
    IRenderContext renderContext { get; }
    IAssetLoader renderAssetLoader { get; }
    
    void Initialize(object graphicDevice);
}