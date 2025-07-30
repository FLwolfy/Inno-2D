namespace InnoInternal.Resource.Impl;

internal interface IShader
{
    string name { get; }
    ShaderStage stage { get; }
}

public enum ShaderInputType
{
    Float, Float2, Float3, Float4
}

public enum ShaderStage
{
    Vertex,
    Fragment
}