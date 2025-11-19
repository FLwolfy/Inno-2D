using System.Security.Cryptography;

namespace Inno.Assets;


public abstract class InnoAsset
{
    public Guid guid { get; internal set; } = Guid.Empty;
    public string sourcePath { get; internal set; } = null!;
    public string sourceHash { get; private set; } = null!;
    
    protected internal InnoAsset() {}
    
    protected internal InnoAsset(Guid guid, string sourcePath)
    {
        this.guid = guid;
        this.sourcePath = sourcePath;
        RecomputeHash();
    }
    
    internal void RecomputeHash()
    {
        using var stream = File.OpenRead(Path.Combine(AssetManager.assetDirectory, sourcePath));
        using var sha = SHA256.Create();
        var hashBytes = sha.ComputeHash(stream);
        sourceHash = Convert.ToHexString(hashBytes);
    }
}