using InnoBase;
using InnoInternal.Resource.Impl;

namespace InnoInternal.Render.Impl;

internal interface IRenderer2D
{
    void Initialize(IRenderCommand renderCommand);
    
    void BeginScene(Matrix viewMatrix, Matrix projectionMatrix);
    void EndScene();
    
    void Clear(Color color);
    void DrawQuad(Matrix transform, IMaterial material, ITexture2D? textureOverride = null);
}