using Inno.Core.Utility;

namespace Inno.Assets;

/// <summary>
/// Maintains a registry of IAssetLoader implementations for different asset types.
/// </summary>
internal static class AssetLoaderRegistry
{
    private static readonly Dictionary<Type, IAssetLoader> LOADERS = new();

    /// <summary>
    /// Initializes the registry by scanning for IAssetLoader implementations.
    /// </summary>
    internal static void SubscribeToTypeCache()
    {
        TypeCacheManager.OnRefreshed += () =>
        {
            LOADERS.Clear();
            
            foreach (var type in TypeCacheManager.GetTypesImplementing<IAssetLoader>())
            {
                Register(type);
            }
        };
        
        TypeCacheManager.Refresh();
    }

    /// <summary>
    /// Registers a loader type.
    /// </summary>
    private static void Register(Type type)
    {
        if (type.IsAbstract || type.IsInterface)
            return;

        var baseType = type.BaseType;
        if (baseType!.IsGenericType && baseType.GetGenericTypeDefinition() == typeof(InnoAssetLoader<>))
        {
            if (Activator.CreateInstance(type) is IAssetLoader instance)
            {
                var genericArg = baseType.GetGenericArguments()[0];
                LOADERS[genericArg] = instance;
            }
        }
    }

    /// <summary>
    /// Tries to get a loader for the specified asset type.
    /// </summary>
    internal static bool TryGetLoader(Type assetType, out IAssetLoader? loader)
    {
        return LOADERS.TryGetValue(assetType, out loader);
    }
}