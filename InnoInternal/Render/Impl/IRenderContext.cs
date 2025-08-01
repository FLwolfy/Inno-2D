using InnoBase;

namespace InnoInternal.Render.Impl;

internal interface IRenderContext
{
    public Matrix viewMatrix { get; set; }
    public Matrix projectionMatrix { get; set; }
    
    void Initialize(IRenderCommand renderCommand);
    
    Vector2 GetWindowSize();
    
    void SetRenderTarget(IRenderTarget? target);
    IRenderTarget? GetRenderTarget();
    IRenderTarget CreateRenderTarget(uint width, uint height);
    
    

}