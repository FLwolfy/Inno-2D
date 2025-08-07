namespace InnoInternal.Render.Impl;

public enum ShaderStage { Vertex, Fragment, Compute }

public interface IShader : IDisposable
{
    ShaderStage Stage { get; }
}