using InnoInternal.Render.Bridge;
using InnoInternal.Resource.Impl;
using Microsoft.Xna.Framework.Graphics;

namespace InnoInternal.Resource.Bridge;

internal class MonoGameAssetLoader : IAssetLoader
{
    private readonly Dictionary<Type, Func<string?, object>> m_loaderMap = new();

    public MonoGameAssetLoader()
    {
        // Add more loaders as needed
        m_loaderMap[typeof(ITexture2D)] = MonoGameTexture2D.LoadFromFile;
        m_loaderMap[typeof(IShader)] = MonoGameShader.LoadFromFile;
    }

    public bool TryLoad<TInterface>(string? path, out TInterface? result)
        where TInterface : class, IAsset
    {
        if (m_loaderMap.TryGetValue(typeof(TInterface), out var loader))
        {
            if (loader(path) is TInterface loaded)
            {
                result = loaded;
                return true;
            }
        }

        result = null;
        return false;
    }
}