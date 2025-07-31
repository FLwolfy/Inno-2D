using InnoInternal.Render.Impl;

namespace InnoInternal.Resource.Impl;

internal interface IMesh
{
    IVertexLayout vertexLayout { get; }
    VertexElement[] vertexBuffer  { get; }
    ushort[] indexCount { get; }
}