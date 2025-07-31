using InnoBase;
using InnoInternal.Resource.Impl;

namespace InnoInternal.Render.Impl;

internal interface IRenderer2D
{
    void Initialize(IRenderCommand renderCommand);
    
    void BeginScene();
    void EndScene();
    
    void Clear(Color color);
    void DrawQuad(Matrix transform, IMaterial2D material, ITexture2D? textureOverride = null);
}