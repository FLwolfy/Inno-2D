namespace Inno.Assets;

internal abstract class InnoAssetLoader<T> : IAssetLoader where T : InnoAsset
{
    public InnoAsset? Load(string relativePath)
    {
        string assetMetaPath = Path.Combine(AssetManager.assetDirectory, relativePath + ".asset");

        if (!File.Exists(assetMetaPath))
        {
            var a = LoadRaw(relativePath, Guid.NewGuid());
            if (a == null) return null;
            
            SaveAsset(a, assetMetaPath);
            return a;
        }

        var yaml  = File.ReadAllText(assetMetaPath);
        var asset = IAssetLoader.DESERIALIZER.Deserialize<T>(yaml); // TODO: Check Deserialization BUG

        string actualSource = Path.Combine(AssetManager.assetDirectory, asset.sourcePath);
        if (!File.Exists(actualSource))
        {
            DeleteAsset(assetMetaPath);
            UnloadRaw(relativePath);
            return null;
        }

        string old = asset.sourceHash;
        asset.RecomputeHash();
        if (old != asset.sourceHash)
        {
            var a = LoadRaw(relativePath, asset.guid);
            if (a == null)
            {
                DeleteAsset(assetMetaPath);
                UnloadRaw(relativePath);
                return null;
            }
            
            SaveAsset(a, assetMetaPath);
            return a;
        }

        return asset;
    }

    private void SaveAsset(T asset, string metaPath)
    {
        string yaml = IAssetLoader.SERIALIZER.Serialize(asset);
        Directory.CreateDirectory(Path.GetDirectoryName(metaPath)!);
        File.WriteAllText(metaPath, yaml);
    }

    private void DeleteAsset(string metaPath)
    {
        if (File.Exists(metaPath))
            File.Delete(metaPath);
    }

    protected abstract T? LoadRaw(string relativePath, Guid guid);
    protected abstract void UnloadRaw(string relativePath);
}