using Inno.Core.Asset.AssetType;

namespace Inno.Core.Asset;

public static class AssetManager
{
    private static string m_rootDirectory = string.Empty;
    
    /// <summary>
    /// Sets the root directory for all asset loading.
    /// Example: "Assets"
    /// </summary>
    public static void SetRootDirectory(string root)
    {
        m_rootDirectory = root.TrimEnd('/', '\\');
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