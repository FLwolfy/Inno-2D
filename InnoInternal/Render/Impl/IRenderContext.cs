using InnoBase;

namespace InnoInternal.Render.Impl;

internal interface IRenderContext
{
    Vector2 GetWindowSize();
    IRenderTarget GetRenderTarget();
    IRenderTarget CreateRenderTarget(uint width, uint height);
    
    public Matrix viewMatrix { get; set; }
    public Matrix projectionMatrix { get; set; }
    public Matrix worldToScreenMatrix { get; }
}