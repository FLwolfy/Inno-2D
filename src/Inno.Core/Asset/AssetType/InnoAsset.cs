namespace Inno.Core.Asset.AssetType;

/// <summary>
/// Base class for all asset types.
/// Holds a unique GUID and the file path.
/// </summary>
public abstract class InnoAsset
{
    /// <summary>
    /// Unique global identifier for this asset.
    /// </summary>
    public Guid guid { get; private set; } = Guid.Empty;

    /// <summary>
    /// Logic to load the asset from the specified path.
    /// Normally overridden by derived classes to handle specific for internal IAsset classes.
    /// </summary>
    protected abstract void OnLoad(string? pathStr);

    /// <summary>
    /// Loads the asset from the specified path and sets its GUID.
    /// If the path is new, a new GUID is generated and registered.
    /// </summary>
    internal void Load(string? pathStr)
    {
        if (pathStr != null)
        {
            if (!AssetRegistry.TryGetGuid(pathStr, out var uid))
            {
                uid = Guid.NewGuid();
                AssetRegistry.Register(uid, pathStr);
            }
            guid = uid;
        }

        OnLoad(pathStr);
    }
}