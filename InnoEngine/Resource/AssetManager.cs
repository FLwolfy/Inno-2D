using InnoEngine.Internal.Resource.Impl;
using InnoEngine.Resource.AssetType;

namespace InnoEngine.Resource;

public static class AssetManager
{
    private static readonly List<IAssetLoader> LOADERS = [];
    private static string m_rootDirectory = string.Empty;

    /// <summary>
    /// Registers a new asset loader.
    /// </summary>
    internal static void RegisterLoader(IAssetLoader registry)
    {
        if (!LOADERS.Contains(registry))
            LOADERS.Add(registry);
    }
    
    /// <summary>
    /// Sets the root directory for all asset loading.
    /// Example: "Assets"
    /// </summary>
    internal static void SetRootDirectory(string root)
    {
        m_rootDirectory = root.TrimEnd('/', '\\');
    }

    /// <summary>
    /// Loads an asset implementation from registered loaders.
    /// </summary>
    internal static TInterface LoadImplFromLoaders<TInterface>(string? path)
        where TInterface : class, IAsset
    {
        if (path == null)
        {
            foreach (var registry in LOADERS)
            {
                if (registry.TryLoad<TInterface>(null, out var result))
                    return result!;
            }
        }
        else
        {
            string fullPath = Path.Combine(m_rootDirectory, path);
            foreach (var registry in LOADERS)
            {
                if (registry.TryLoad<TInterface>(fullPath, out var result))
                    return result!;
            }
        }
        
        throw new Exception($"AssetManager: Cannot load {typeof(TInterface).Name} from: {path}");
    }

    /// <summary>
    /// Loads an asset of type T from the specified path.
    /// </summary>
    /// <param name="path">The path of the asset</param>
    public static T Load<T>(string path) where T : InnoAsset
    {
        var asset = (T)Activator.CreateInstance(typeof(T), nonPublic: true)!;
        asset.Load(path);
        return asset;
    }
    
    /// <summary>
    /// Creates a default asset of type T without loading from a path.
    /// </summary>
    public static T Create<T>() where T : InnoAsset
    {
        return Load<T>(null!);
    }
}