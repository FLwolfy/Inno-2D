using InnoBase;

namespace InnoInternal.Render.Impl;

internal interface IRenderContext
{
    Vector2 GetWindowSize();
    Vector2 GetRenderTargetSize();
    
    /// <summary>
    /// This is also called as the viewProjection matrix (or the projectionView matrix, depending on RenderAPIs).
    /// </summary>
    public Matrix? cameraMatrix { get; set; }
    
    void BeginFrame();
    void EndFrame();
}