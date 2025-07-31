using InnoBase;

namespace InnoInternal.Render.Impl;

internal interface IRenderCommand
{
    void Begin();
    void End();
    
    void SetRenderTarget(IRenderTarget? target);
    void DrawIndexed(uint indexCount, uint instanceCount = 1, uint indexStart = 0, int vertexOffset = 0, uint instanceStart = 0);
    void Clear(Color color);
}