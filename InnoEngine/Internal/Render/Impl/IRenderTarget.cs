using InnoEngine.Internal.Base;
using InnoEngine.Internal.Resource.Impl;

namespace InnoEngine.Internal.Render.Impl;

internal interface IRenderTarget
{
    void Bind();
    void Unbind();
    void Clear(Color clearColor);  
    ITexture2D GetColorTexture();
}