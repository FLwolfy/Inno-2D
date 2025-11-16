using System.Collections.Concurrent;
using System.Reflection;

using Inno.Platform.Graphics;

namespace Inno.Graphics.Resources;

public static class ShaderLibrary
{
    private static readonly Assembly ASSEMBLY = typeof(ShaderLibrary).GetTypeInfo().Assembly;
    private static readonly ConcurrentDictionary<string, Shader> CACHE = new();

    public static Shader LoadEmbeddedShader(string fullName)
    {
        if (CACHE.TryGetValue(fullName, out var cached))
            return cached;
        
        int dotIndex = fullName.LastIndexOf('.');
        string extension = fullName.Substring(dotIndex + 1);

        var resources = ASSEMBLY.GetManifestResourceNames();
        var match = resources.FirstOrDefault(r => r.EndsWith(fullName, StringComparison.OrdinalIgnoreCase));
        if (match == null) throw new FileNotFoundException($"Embedded resource '{fullName}' not found.");

        using Stream s = ASSEMBLY.GetManifestResourceStream(match)!;
        using StreamReader reader = new StreamReader(s);
        
        var shader = new Shader
        (
            fullName.Substring(0, dotIndex), 
            GetShaderStageFromExt(extension), 
            reader.ReadToEnd()
        );
        
        CACHE[fullName] = shader;
        return shader;
    }

    private static ShaderStage GetShaderStageFromExt(string extension)
    {
        return extension switch
        {
            "vert" => ShaderStage.Vertex,
            "frag" => ShaderStage.Fragment,
            ".comp" => ShaderStage.Compute,
            _ => throw new NotSupportedException($"Unsupported shader file extenstion: {extension}")
        };
    }

    public static void ClearCache()
    {
        CACHE.Clear();
    }
}