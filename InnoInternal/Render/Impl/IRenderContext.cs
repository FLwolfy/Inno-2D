using InnoBase;

namespace InnoInternal.Render.Impl;

internal interface IRenderContext
{
    void Initialize(IRenderCommand renderCommand);
    
    // Camera properties
    public Matrix viewMatrix { get; }
    public Matrix projectionMatrix { get; }
    
    // Render target
    IRenderTarget? renderTarget { get; }
    IRenderTarget CreateRenderTarget(uint width, uint height);
}