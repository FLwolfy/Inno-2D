using InnoEngine.Internal.Base;

namespace InnoEngine.Internal.Render.Impl;

internal interface IRenderTarget
{
    void Bind();
    void Unbind();
    void Clear(Color clearColor);  
    ITexture2D GetColorTexture();
}