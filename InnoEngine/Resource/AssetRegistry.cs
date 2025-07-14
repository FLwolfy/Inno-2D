using System.Text.Json;

namespace InnoEngine.Resource;

public static class AssetRegistry
{
    private static readonly Dictionary<Guid, string> GUID_TO_PATH = new();
    private static readonly Dictionary<string, Guid> PATH_TO_GUID = new();

    private const string c_registryFile = "AssetRegistry.json";

    public static void Register(Guid guid, string path)
    {
        GUID_TO_PATH[guid] = path;
        PATH_TO_GUID[path] = guid;
    }

    public static bool TryGetPath(Guid guid, out string? path) =>
        GUID_TO_PATH.TryGetValue(guid, out path);

    public static bool TryGetGuid(string path, out Guid guid) =>
        PATH_TO_GUID.TryGetValue(path, out guid);

    public static void SaveToDisk()
    {
        var dict = new Dictionary<string, string>();
        foreach (var kv in PATH_TO_GUID)
            dict[kv.Key] = kv.Value.ToString();

        var json = JsonSerializer.Serialize(dict, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(c_registryFile, json);
    }

    public static void LoadFromDisk()
    {
        if (!File.Exists(c_registryFile))
            return;

        var json = File.ReadAllText(c_registryFile);
        var dict = JsonSerializer.Deserialize<Dictionary<string, string>>(json)!;

        PATH_TO_GUID.Clear();
        GUID_TO_PATH.Clear();

        foreach (var kv in dict)
        {
            var path = kv.Key;
            var guid = Guid.Parse(kv.Value);
            PATH_TO_GUID[path] = guid;
            GUID_TO_PATH[guid] = path;
        }
    }
}