namespace InnoInternal.Resource.Impl;

internal interface IShader
{
    ShaderState shaderState { get; }
}

internal enum ShaderState
{
    Vertex,
    Fragment,
    Compute
}