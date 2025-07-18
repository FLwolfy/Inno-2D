using InnoBase;

namespace InnoInternal.Render.Impl;

internal interface IRenderCommand
{
    void Clear(Color color);
    void SetViewport(Rect? viewport);
    void SetRenderTarget(IRenderTarget? target);
    IRenderTarget CreateRenderTarget(int width, int height);
}