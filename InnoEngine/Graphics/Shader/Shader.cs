using InnoBase.Graphics;

namespace InnoEngine.Graphics.Shader;

public class Shader(string name, ShaderStage stage, string sourceCode)
{
    public string name { get; } = name;
    public ShaderStage stage { get; } = stage;
    public string sourceCode { get; } = sourceCode;
}