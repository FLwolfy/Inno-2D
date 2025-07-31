using InnoInternal.Render.Impl;
using InnoInternal.Resource.Impl;


namespace InnoInternal.Resource.Bridge;

internal class VeldridAssetLoader : IAssetLoader
{
    private IRenderCommand m_command = null!;
    
    private readonly Dictionary<Type, Func<IRenderCommand, string?, object>> m_fileLoaderMap = new();
    
    public void Initialize(IRenderCommand renderCommand)
    {
        m_command = renderCommand;
        
        RegisterLoaderMap();
    }

    private void RegisterLoaderMap()
    {
        m_fileLoaderMap[typeof(ITexture2D)] = VeldridTexture2D.LoadFromFile;
        m_fileLoaderMap[typeof(IShader)] = VeldridShader.LoadFromFile;
        m_fileLoaderMap[typeof(IMaterial2D)] = VeldridMaterial2D.LoadFromFile;
    }

    public bool TryLoad<TInterface>(string? path, out TInterface? result)
        where TInterface : class, IAsset
    {
        if (m_fileLoaderMap.TryGetValue(typeof(TInterface), out var loader))
        {
            if (loader(m_command, path) is TInterface loaded)
            {
                result = loaded;
                return true;
            }
        }

        result = null;
        return false;
    }
}