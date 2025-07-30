using InnoInternal.Resource.Impl;

using Veldrid;

namespace InnoInternal.Resource.Bridge;

internal class VeldridAssetLoader : IAssetLoader
{
    private readonly GraphicsDevice m_device;
    
    private readonly Dictionary<Type, Func<GraphicsDevice, string?, object>> m_fileLoaderMap = new();
    
    public VeldridAssetLoader(GraphicsDevice device)
    {
        m_device = device;

        RegisterLoaderMap();
    }

    private void RegisterLoaderMap()
    {
        m_fileLoaderMap[typeof(ITexture2D)] = VeldridTexture2D.LoadFromFile;
        m_fileLoaderMap[typeof(IShader)]    = VeldridShader.LoadFromFile;
    }

    public bool TryLoad<TInterface>(string? path, out TInterface? result)
        where TInterface : class, IAsset
    {
        if (m_fileLoaderMap.TryGetValue(typeof(TInterface), out var loader))
        {
            if (loader(m_device, path) is TInterface loaded)
            {
                result = loaded;
                return true;
            }
        }

        result = null;
        return false;
    }
}