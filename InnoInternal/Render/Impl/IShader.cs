namespace InnoInternal.Render.Impl;

[Flags]
public enum ShaderStage : byte
{
    None = 0,
    /// <summary>
    /// The vertex shader stage.
    /// </summary>
    Vertex = 1 << 0,
    /// <summary>
    /// The geometry shader stage.
    /// </summary>
    Geometry = 1 << 1,
    /// <summary>
    /// The tessellation control (or hull) shader stage.
    /// </summary>
    TessellationControl = 1 << 2,
    /// <summary>
    /// The tessellation evaluation (or domain) shader stage.
    /// </summary>
    TessellationEvaluation = 1 << 3,
    /// <summary>
    /// The fragment (or pixel) shader stage.
    /// </summary>
    Fragment = 1 << 4,
    /// <summary>
    /// The compute shader stage.
    /// </summary>
    Compute = 1 << 5,
}

public struct ShaderDescription
{
    public ShaderStage stage;
    public string sourceCode;
}

public interface IShader : IDisposable
{
    ShaderStage stage { get; }
}