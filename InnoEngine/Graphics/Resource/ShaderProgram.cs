using InnoBase.Graphics;

namespace InnoEngine.Graphics.Resource;

public class ShaderProgram
{
    private readonly Dictionary<ShaderStage, Dictionary<string, Resource.Shader>> m_shaders = new();

    public void Add(Resource.Shader shader)
    {
        if (!m_shaders.TryGetValue(shader.stage, out var stageDict))
        {
            stageDict = new Dictionary<string, Resource.Shader>();
            m_shaders[shader.stage] = stageDict;
        }

        stageDict[shader.name] = shader;
    }

    public Resource.Shader? Get(ShaderStage stage, string name)
    {
        if (m_shaders.TryGetValue(stage, out var stageDict))
        {
            if (stageDict.TryGetValue(name, out var shader))
                return shader;
        }
        return null;
    }

    public IReadOnlyDictionary<string, Resource.Shader> GetShadersByStage(ShaderStage stage) 
        => m_shaders.TryGetValue(stage, out var stageDict) ? stageDict : new Dictionary<string, Resource.Shader>();
    
    public bool Contains(ShaderStage stage, string name) 
        => m_shaders.TryGetValue(stage, out var stageDict) && stageDict.ContainsKey(name);
}
