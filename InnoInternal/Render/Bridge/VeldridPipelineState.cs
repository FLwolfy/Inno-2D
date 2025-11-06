using System.Reflection;
using InnoBase;
using InnoInternal.Render.Impl;
using Veldrid;

using InnoPSDescription = InnoInternal.Render.Impl.PipelineStateDescription;
using VeldridPSDescription = Veldrid.GraphicsPipelineDescription;

namespace InnoInternal.Render.Bridge;

internal class VeldridPipelineState : IPipelineState
{
    private readonly GraphicsDevice m_graphicsDevice;
    private VeldridPSDescription m_innerDescription;

    internal Pipeline inner { get; }

    public VeldridPipelineState(GraphicsDevice graphicsDevice, InnoPSDescription desc)
    {
        m_graphicsDevice = graphicsDevice;

        m_innerDescription = ToVeldridPSDesc(desc);
        inner = m_graphicsDevice.ResourceFactory.CreateGraphicsPipeline(ref m_innerDescription);
    }
    
    internal void SetFrameBufferOutputDescription(OutputDescription desc)
    {
        m_innerDescription.Outputs = desc;
    }

    public void Dispose()
    {
        inner.Dispose();
    }

    private VeldridPSDescription ToVeldridPSDesc(InnoPSDescription desc)
    {
        var vertexShader = ((VeldridShader)desc.vertexShader).inner;
        var fragmentShader = ((VeldridShader)desc.fragmentShader).inner;
        var vertexLayoutDescPair = new[] { GenerateVertexLayoutFromType(desc.vertexLayoutType) };
        var resourceLayouts = desc.resourceSetBindings.Length > 0 
            ? desc.resourceSetBindings
                .Select(t => m_graphicsDevice.ResourceFactory.CreateResourceLayout(VeldridResourceSet.GenerateResourceLayoutFromBinding(t)))
                .ToArray() 
            : [];

        return new GraphicsPipelineDescription
        {
            BlendState = BlendStateDescription.SingleAlphaBlend,
            DepthStencilState = new DepthStencilStateDescription(false, false, ComparisonKind.Always),
            RasterizerState = RasterizerStateDescription.CullNone,
            PrimitiveTopology = PrimitiveTopology.TriangleList,
            ShaderSet = new ShaderSetDescription(vertexLayoutDescPair, [vertexShader, fragmentShader]),
            ResourceLayouts = resourceLayouts,
            Outputs = m_graphicsDevice.SwapchainFramebuffer.OutputDescription
        };
    }
    
    private static VertexLayoutDescription GenerateVertexLayoutFromType(Type t)
    {
        var fields = t.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)
            .OrderBy(f => f.MetadataToken);
        var elements = fields.Select(f =>
        {
            if (f.FieldType == typeof(Vector2))
                return new VertexElementDescription(f.Name, VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2);
            if (f.FieldType == typeof(Vector3))
                return new VertexElementDescription(f.Name, VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float3);
            if (f.FieldType == typeof(Vector4))
                return new VertexElementDescription(f.Name, VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float4);
            if (f.FieldType == typeof(Color))
                return new VertexElementDescription(f.Name, VertexElementSemantic.Color, VertexElementFormat.Float4);

            throw new NotSupportedException($"Unsupported vertex field type: {f.FieldType}");
        }).ToArray();

        return new VertexLayoutDescription(elements);
    }
}
