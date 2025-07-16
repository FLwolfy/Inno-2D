using InnoBase;
using InnoInternal.Resource.Impl;

namespace InnoInternal.Render.Impl;

internal interface IRenderTarget : IDisposable
{
    void Bind();
    void Unbind();
    void Clear(Color clearColor);  
    ITexture2D GetColorTexture();
}