using InnoBase;
using InnoInternal.Resource.Impl;

namespace InnoInternal.Render.Impl;

internal interface IRenderCommand
{
    void Initialize(object graphicDevice);
    
    // Lifecycle
    void Begin(Matrix viewMatrix, Matrix projectionMatrix);
    void End();
    
    // Draw
    void Clear(Color color);
    void DrawMesh(IMesh mesh, IMaterial material, ITexture2D? textureOverride = null);
    
    // Render View
    void SetRenderTarget(IRenderTarget? target);
    void SetViewPort(Vector4 viewport);
}