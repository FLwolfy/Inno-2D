using InnoBase.Graphics;

namespace InnoInternal.Render.Impl;

public struct ShaderDescription
{
    public ShaderStage stage;
    public string sourceCode;
}

public interface IShader : IDisposable
{
    ShaderStage stage { get; }
}