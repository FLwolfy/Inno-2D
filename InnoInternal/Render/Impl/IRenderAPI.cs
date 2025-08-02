using InnoInternal.ImGui.Impl;
using InnoInternal.Resource.Impl;

namespace InnoInternal.Render.Impl;

internal interface IRenderAPI
{
    IRenderer2D renderer2D { get; }
    IImGuiRenderer rendererGUI { get; }
    IRenderContext renderContext { get; }
    IAssetLoader renderAssetLoader { get; }
    
    void Initialize(object graphicDevice, object windowHolder);
}