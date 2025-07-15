using InnoInternal.Resource.Impl;
using Microsoft.Xna.Framework.Graphics;

namespace InnoInternal.Resource.Bridge;

internal class MonoGameAssetLoader : IAssetLoader
{
    private readonly Dictionary<Type, Func<string?, object>> m_loaderMap = new();
    
    public void Initialize(object data)
    {
        if (data is not GraphicsDevice device)
        {
            throw new ArgumentException("Invalid data type. Expected GraphicsDevice.", nameof(data));
        }
        
        // Add more loaders as needed
        m_loaderMap[typeof(ITexture2D)] = path => MonoGameTexture2D.LoadFromFile(device, path);
        m_loaderMap[typeof(IShader)] = path => MonoGameShader.LoadFromFile(device, path);
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