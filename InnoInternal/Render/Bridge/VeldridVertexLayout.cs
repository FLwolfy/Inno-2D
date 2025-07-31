using InnoInternal.Render.Impl;
using InnoInternal.Resource.Bridge;

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
                VeldridShader.GetFormatFromType(e.fieldType)
            );
        }

        layoutDescription = new VertexLayoutDescription(veldridElements);
    }

    
}