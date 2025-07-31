using InnoBase;

namespace InnoInternal.Render.Impl;

internal interface IRenderContext
{
    void Initialize(IRenderCommand renderCommand);
    
    Vector2 GetWindowSize();
    
    void SetRenderTarget(IRenderTarget? target);
    IRenderTarget? GetRenderTarget();
    IRenderTarget CreateRenderTarget(uint width, uint height);
    
    
    public Matrix viewMatrix { get; set; }
    public Matrix projectionMatrix { get; set; }
}