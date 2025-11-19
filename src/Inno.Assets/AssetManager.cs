namespace Inno.Assets;

public static class AssetManager
{
    private static readonly Dictionary<string, Guid> PATH_GUID_PAIRS = new();
    private static readonly Dictionary<Guid, InnoAsset> LOADED_ASSETS = new();
    private static FileSystemWatcher? m_watcher;

    public static string libraryDirectory { get; private set; } = null!;
    public static string assetDirectory { get; private set; } = null!;

    public static void Initialize(string assetDir, string libraryDir)
    {
        assetDirectory = assetDir;
        libraryDirectory = libraryDir;

        m_watcher = new FileSystemWatcher(assetDir)
        {
            IncludeSubdirectories = true,
            NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName
        };
        m_watcher.Changed += OnFileChanged;
        m_watcher.Deleted += OnFileChanged;
        m_watcher.EnableRaisingEvents = true;

        AssetLoaderRegistry.SubscribeToTypeCache();
    }

    public static T? Load<T>(string relativePath) where T : InnoAsset
    {
        if (!AssetLoaderRegistry.TryGetLoader(typeof(T), out var loader))
            return null;

        var loaded = loader!.Load(relativePath);
        if (loaded == null) return null;
        
        string absSource = Path.GetFullPath(Path.Combine(assetDirectory, loaded.sourcePath));

        PATH_GUID_PAIRS[absSource] = loaded.guid;
        LOADED_ASSETS[loaded.guid] = loaded;

        return (T)loaded;
    }

    private static void OnFileChanged(object sender, FileSystemEventArgs e)
    {
        string abs = Path.GetFullPath(e.FullPath);

        if (!PATH_GUID_PAIRS.TryGetValue(abs, out var guid))
            return;

        if (!LOADED_ASSETS.TryGetValue(guid, out var existing))
            return;

        if (!AssetLoaderRegistry.TryGetLoader(existing.GetType(), out var loader))
            return;

        var reloaded = loader!.Load(existing.sourcePath);
        if (reloaded == null)
        {
            PATH_GUID_PAIRS.Remove(abs);
            LOADED_ASSETS.Remove(guid);
            return;
        }

        LOADED_ASSETS[guid] = reloaded;
    }
}
