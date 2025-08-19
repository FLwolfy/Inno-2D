namespace InnoInternal.Render.Impl;

public enum ShaderStage { Vertex, Fragment, Compute }

public struct ShaderDescription
{
    public ShaderStage stage;
    public string sourceCode;
    public string entryPoint;
}

public interface IShader : IDisposable
{
    ShaderStage stage { get; }
}