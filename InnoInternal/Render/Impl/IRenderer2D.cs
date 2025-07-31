using InnoBase;
using InnoInternal.Resource.Impl;

namespace InnoInternal.Render.Impl;

internal interface IRenderer2D
{
    void BeginScene();
    void EndScene();

    void BeginPass(string name);
    void EndPass();
    
    void DrawQuad(Matrix transform, IMaterial2D material, ITexture2D? textureOverride = null);
}