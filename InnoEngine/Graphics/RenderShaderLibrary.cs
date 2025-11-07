using System.Reflection;
using System.Collections.Concurrent;

namespace InnoEngine.Graphics;

public static class RenderShaderLibrary
{
    private static readonly Assembly ASSEMBLY = typeof(RenderShaderLibrary).GetTypeInfo().Assembly;
    private static readonly ConcurrentDictionary<string, string> CACHE = new();

    public static string GetEmbeddedShaderCode(string shortName)
    {
        if (CACHE.TryGetValue(shortName, out var cached))
            return cached;

        var resources = ASSEMBLY.GetManifestResourceNames();
        var match = resources.FirstOrDefault(r => r.EndsWith(shortName, StringComparison.OrdinalIgnoreCase));
        if (match == null) throw new FileNotFoundException($"Embedded resource '{shortName}' not found.");

        using Stream s = ASSEMBLY.GetManifestResourceStream(match)!;
        using StreamReader reader = new StreamReader(s);
        var code = reader.ReadToEnd();

        CACHE[shortName] = code;
        return code;
    }

    public static void ClearCache()
    {
        CACHE.Clear();
    }
}