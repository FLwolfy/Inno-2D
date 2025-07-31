using InnoBase;

namespace InnoInternal.Render.Impl;

internal interface IRenderCommand
{
    void Clear(Color color);
    void SetRenderTarget(IRenderTarget? target);
}