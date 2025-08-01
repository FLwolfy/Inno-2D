using InnoBase;

namespace InnoInternal.Resource.Impl;

public interface IMesh : IAsset
{
    Vector2[] positions { get; }
    Color[]? colors { get; }
    Vector2[]? uvs { get; }
    ushort[] indices { get; }
}