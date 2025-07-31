using InnoBase;
using InnoInternal.Render.Impl;

using Veldrid;

namespace InnoInternal.Render.Bridge;

internal class VeldridVertexLayout : IVertexLayout
{
    public VertexLayoutDescription layoutDescription;
    
    public void Build(VertexElement[] elements)
    {
        var veldridElements = new VertexElementDescription[elements.Length];

        for (int i = 0; i < elements.Length; i++)
        {
            var e = elements[i];
            veldridElements[i] = new VertexElementDescription(
                e.semanticName,
                VertexElementSemantic.TextureCoordinate, // This makes no effect
                GetFormatFromType(e.fieldType)
            );
        }

        layoutDescription = new VertexLayoutDescription(veldridElements);
    }

    private VertexElementFormat GetFormatFromType(Type type)
    {
        if (type == typeof(Vector2)) return VertexElementFormat.Float2;
        if (type == typeof(Vector3)) return VertexElementFormat.Float3;
        if (type == typeof(Vector4)) return VertexElementFormat.Float4;
        if (type == typeof(Color)) return VertexElementFormat.Float4;
        if (type == typeof(uint)) return VertexElementFormat.UInt1;

        throw new NotSupportedException($"Unsupported vertex element type: {type}");
    }
}