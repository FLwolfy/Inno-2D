namespace InnoInternal.Resource.Impl;

internal interface IAssetLoader
{
    bool TryLoad<TInterface>(string? path, out TInterface? result) where TInterface : class, IAsset;
}