using InnoBase;
using InnoInternal.Render.Impl;
using InnoInternal.Resource.Impl;
using InnoInternal.Resource.Bridge;

using Veldrid;

namespace InnoInternal.Render.Bridge;

internal class VeldridRenderer2D : IRenderer2D
{
    private readonly GraphicsDevice m_device;
    private readonly CommandList m_commandList;

    private Pipeline m_pipeline;
    private ResourceLayout m_resourceLayout;

    public VeldridRenderer2D(GraphicsDevice device, VeldridRenderCommand command)
    {
        m_device = device;
        m_commandList = command.commandList;

        CreatePipeline();
    }

    private void CreatePipeline()
    {
        var factory = m_device.ResourceFactory;

        m_resourceLayout = factory.CreateResourceLayout(new ResourceLayoutDescription(
            new ResourceLayoutElementDescription("Texture", ResourceKind.TextureReadOnly, ShaderStages.Fragment),
            new ResourceLayoutElementDescription("Sampler", ResourceKind.Sampler, ShaderStages.Fragment)
        ));

        Shader[] shaders = VeldridShaderLibrary.LoadShaderSet(m_device, "Quad2D"); // 你需要提供这个

        m_pipeline = factory.CreateGraphicsPipeline(new GraphicsPipelineDescription
        {
            BlendState = BlendStateDescription.SingleAlphaBlend,
            DepthStencilState = DepthStencilStateDescription.Disabled,
            RasterizerState = new RasterizerStateDescription(
                FaceCullMode.None, PolygonFillMode.Solid, FrontFace.Clockwise, true, false),
            PrimitiveTopology = PrimitiveTopology.TriangleList,
            ResourceLayouts = new[] { m_resourceLayout },
            ShaderSet = new ShaderSetDescription(
                vertexLayouts: new[] {
                    new VertexLayoutDescription(
                        new VertexElementDescription("Position", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2),
                        new VertexElementDescription("TexCoord", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2)
                    )
                },
                shaders: shaders
            ),
            Outputs = m_device.SwapchainFramebuffer.OutputDescription
        });
    }

    public void Begin()
    {
        m_commandList.SetFramebuffer(VeldridRenderAPI.CurrentTarget?.rawFramebuffer ?? m_device.SwapchainFramebuffer);
        m_commandList.SetPipeline(m_pipeline);
    }

    public void DrawQuad(Rect destinationRect, Rect? sourceRect, ITexture2D texture, Color color, float rotation = 0, float layerDepth = 0, Vector2? origin = null)
    {
        if (texture is not VeldridTexture2D veldridTex)
            throw new ArgumentException("Texture must be VeldridTexture2D");

        // Vertex data: 2D quad with position and UV
        var pos = destinationRect;
        var uv = sourceRect ?? new Rect(0, 0, veldridTex.Width, veldridTex.Height);
        var originVec = origin ?? new Vector2(0, 0);

        var verts = new[]
        {
            new Vertex(pos.x, pos.y, uv.x / veldridTex.Width, uv.y / veldridTex.Height),
            new Vertex(pos.x + pos.width, pos.y, (uv.x + uv.width) / veldridTex.Width, uv.y / veldridTex.Height),
            new Vertex(pos.x + pos.width, pos.y + pos.height, (uv.x + uv.width) / veldridTex.Width, (uv.y + uv.height) / veldridTex.Height),
            new Vertex(pos.x, pos.y, uv.x / veldridTex.Width, uv.y / veldridTex.Height),
            new Vertex(pos.x + pos.width, pos.y + pos.height, (uv.x + uv.width) / veldridTex.Width, (uv.y + uv.height) / veldridTex.Height),
            new Vertex(pos.x, pos.y + pos.height, uv.x / veldridTex.Width, (uv.y + uv.height) / veldridTex.Height),
        };

        var vertexBuffer = m_device.ResourceFactory.CreateBuffer(new BufferDescription((uint)(6 * sizeof(float) * 4), BufferUsage.VertexBuffer));
        m_commandList.UpdateBuffer(vertexBuffer, 0, verts);

        var resourceSet = m_device.ResourceFactory.CreateResourceSet(new ResourceSetDescription(
            m_resourceLayout,
            veldridTex.rawTextureView,
            veldridTex.rawSampler
        ));

        m_commandList.SetGraphicsResourceSet(0, resourceSet);
        m_commandList.SetVertexBuffer(0, vertexBuffer);
        m_commandList.Draw((uint)verts.Length);
    }

    public void End()
    {
        // nothing now
    }

    private readonly record struct Vertex(float x, float y, float u, float v);
}
