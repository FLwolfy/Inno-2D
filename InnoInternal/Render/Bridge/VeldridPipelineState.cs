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
    private readonly InnoPSDescription m_pipelineDesc;
    
    internal Dictionary<OutputDescription, Pipeline> inner { get; }

    public VeldridPipelineState(GraphicsDevice graphicsDevice, InnoPSDescription desc)
    {
        m_graphicsDevice = graphicsDevice;
        m_pipelineDesc = desc;

        inner = new Dictionary<OutputDescription, Pipeline>();
        inner[graphicsDevice.SwapchainFramebuffer.OutputDescription] = graphicsDevice.ResourceFactory.CreateGraphicsPipeline(ToVeldridPSDesc(desc, graphicsDevice.SwapchainFramebuffer.OutputDescription));
    }
    
    internal void ValidateFrameBufferOutputDesc(OutputDescription outputDesc)
    {
        if (inner.ContainsKey(outputDesc)) return;
        inner[outputDesc] = m_graphicsDevice.ResourceFactory.CreateGraphicsPipeline(ToVeldridPSDesc(m_pipelineDesc, outputDesc));
    }

    private VeldridPSDescription ToVeldridPSDesc(InnoPSDescription desc, OutputDescription outputDesc)
    {
        var vertexShader = ((VeldridShader)desc.vertexShader).inner;
        var fragmentShader = ((VeldridShader)desc.fragmentShader).inner;
        var vertexLayoutDescPair = new[] { GenerateVertexLayoutFromType(desc.vertexLayoutType) };
        var depthStencilState = ToVeldridDepthDesc(desc.depthStencilState);
        var resourceLayouts = desc.resourceLayoutSpecifiers?.Length > 0 
            ? desc.resourceLayoutSpecifiers
                .Select(t => m_graphicsDevice.ResourceFactory.CreateResourceLayout(VeldridResourceSet.GenerateResourceLayoutFromBinding(t)))
                .ToArray() 
            : [];

        // TODO: Handle customized RasterizerState
        var rasterizerState = new RasterizerStateDescription(
            FaceCullMode.None,
            PolygonFillMode.Solid,
            FrontFace.Clockwise,
            depthClipEnabled: true,
            scissorTestEnabled: true
        );

        return new GraphicsPipelineDescription
        {
            BlendState = BlendStateDescription.SingleAlphaBlend,
            DepthStencilState = depthStencilState,
            RasterizerState = rasterizerState,
            PrimitiveTopology = PrimitiveTopology.TriangleList,
            ShaderSet = new ShaderSetDescription(vertexLayoutDescPair, [vertexShader, fragmentShader]),
            ResourceLayouts = resourceLayouts,
            Outputs = outputDesc
        };
    }

    private static DepthStencilStateDescription ToVeldridDepthDesc(DepthStencilState dss)
    {
        return dss switch
        {
            DepthStencilState.Disabled => DepthStencilStateDescription.Disabled,
            DepthStencilState.DepthOnlyLessEqual => DepthStencilStateDescription.DepthOnlyLessEqual,
            DepthStencilState.DepthOnlyGreaterEqual => DepthStencilStateDescription.DepthOnlyGreaterEqual,
            DepthStencilState.DepthReadOnlyLessEqual => DepthStencilStateDescription.DepthOnlyLessEqualRead,
            DepthStencilState.DepthReadOnlyGreaterEqual => DepthStencilStateDescription.DepthOnlyGreaterEqualRead,
            _ => throw new NotSupportedException($"Unsupported depth stencil state: {dss}")
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
    
    public void Dispose()
    {
        foreach (var p in inner.Values)
        {
            p.Dispose();
        }
    }
}
