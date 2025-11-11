namespace InnoBase.Graphics;

[Flags]
public enum ShaderStage : byte
{
    // NOTE: This is copied from Veldrid.ShaderStages.
    //       Values must match exactly.
    None = 0,
    Vertex = 1 << 0,
    Geometry = 1 << 1,
    TessellationControl = 1 << 2,
    TessellationEvaluation = 1 << 3,
    Fragment = 1 << 4,
    Compute = 1 << 5,
}