using InnoBase;

namespace InnoInternal.Render.Impl;

internal interface IRenderContext
{
    Vector2 GetWindowSize();
    Vector2 GetCurrentRenderTargetSize();
    
    public Matrix viewMatrix { get; set; }
    public Matrix projectionMatrix { get; set; }
    public Matrix worldToScreenMatrix { get; }
}