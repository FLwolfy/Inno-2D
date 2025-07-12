using InnoEngine.Internal.Base;

namespace InnoEngine.Internal.Render.Impl;

internal interface IRenderCommand
{
    void Clear(Color color);
    void SetViewport(Rect viewport);
    void SetRenderTarget(IRenderTarget? target);
    IRenderTarget CreateRenderTarget(int width, int height);
}