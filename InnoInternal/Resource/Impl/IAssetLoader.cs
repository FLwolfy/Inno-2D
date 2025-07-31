using InnoInternal.Render.Impl;

namespace InnoInternal.Resource.Impl;

internal interface IAssetLoader
{
    void Initialize(IRenderCommand renderCommand);
    
    bool TryLoad<TInterface>(string? path, out TInterface? result) where TInterface : class, IAsset;
}