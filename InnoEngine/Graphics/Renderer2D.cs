using InnoBase;
using InnoEngine.Graphics.RenderObject;
using InnoInternal.Render.Impl;

namespace InnoEngine.Graphics;

public class Renderer2D : IDisposable
{
    private readonly IGraphicsDevice m_graphicsDevice;
    private readonly ICommandList m_commandList;
    
    // World info
    public Matrix viewProjection { get; private set; }

    // Quad Resources
    private Mesh m_quadMesh = null!;
    private Material m_quadMaterial = null!;
    private IResourceSet m_quadResources = null!;

    public Renderer2D(IGraphicsDevice graphicsDevice)
    {
        m_graphicsDevice = graphicsDevice;
        m_commandList = graphicsDevice.CreateCommandList();
    }
    
    public void LoadResources()
    {
        CreateQuadResources();
    }

    private void CreateQuadResources()
    {
        // Buffers
        RenderVertexLayout.VertexPosition[] quadVertices =
        [
            new(new Vector3(-1.0f, 1.0f, 0f)),
            new(new Vector3(1.0f, 1.0f, 0f)),
            new(new Vector3(-1.0f, -1.0f, 0f)),
            new(new Vector3(1.0f, -1.0f, 0f))
        ];

        var quadIndices = new short[] { 0, 1, 2, 1, 3, 2 };
        var vertexSize = sizeof(float) * 3;

        var vb = m_graphicsDevice.CreateVertexBuffer((uint)(quadVertices.Length * vertexSize));
        var ib = m_graphicsDevice.CreateIndexBuffer((uint)(quadIndices.Length * sizeof(ushort)));
        
        var mvpBuffer = m_graphicsDevice.CreateUniformBuffer<Matrix>("MVP");
        var colorBuffer = m_graphicsDevice.CreateUniformBuffer<Color>("Color");

        vb.Set(quadVertices);
        ib.Set(quadIndices);

        // Shaders
        var vertexDesc = new ShaderDescription
        {
            stage = ShaderStage.Vertex,
            sourceCode = RenderShaderLibrary.GetEmbeddedShaderCode("SolidQuad.vert")
        };
        var fragmentDesc = new ShaderDescription
        {
            stage = ShaderStage.Fragment,
            sourceCode = RenderShaderLibrary.GetEmbeddedShaderCode("SolidQuad.frag")
        };

        var (vertexShader, fragmentShader) = m_graphicsDevice.CreateVertexFragmentShader(vertexDesc, fragmentDesc);
        
        // Resource
        var resourceSetBinding = new ResourceSetBinding
        {
            shaderStages = ShaderStage.Vertex,
            uniformBuffers = [mvpBuffer, colorBuffer]
        };
        m_quadResources = m_graphicsDevice.CreateResourceSet(resourceSetBinding);

        // Pipeline
        var pipelineDesc = new PipelineStateDescription
        {
            vertexShader = vertexShader,
            fragmentShader = fragmentShader,
            vertexLayoutType = typeof(RenderVertexLayout.VertexPosition),
            resourceSetBindings = [resourceSetBinding]
        };
        var pipeline = m_graphicsDevice.CreatePipelineState(pipelineDesc);

        // Mesh and Material
        m_quadMesh = new Mesh(vb, ib);
        m_quadMaterial = new Material(vertexShader, fragmentShader, pipeline, [mvpBuffer, colorBuffer]);
    }
    
    public void DrawQuad(Matrix transform, Color color)
    {
        var mvp = transform * viewProjection;
        
        m_commandList.UpdateUniform(m_quadMaterial.uniformBuffers["MVP"], ref mvp);
        m_commandList.UpdateUniform(m_quadMaterial.uniformBuffers["Color"], ref color);
        
        m_commandList.SetPipelineState(m_quadMaterial.pipeline);
        m_commandList.SetVertexBuffer(m_quadMesh.vertexBuffer);
        m_commandList.SetIndexBuffer(m_quadMesh.indexBuffer);
        m_commandList.SetResourceSet(0, m_quadResources);
        m_commandList.DrawIndexed(6);
    }
    
    public void ClearColor(Color color)
    {
        m_commandList.ClearColor(color);
    }

    public void BeginFrame(Matrix viewProjectionMatrix, IFrameBuffer target)
    {
        viewProjection = viewProjectionMatrix;
        
        m_commandList.Begin();
        m_commandList.SetFrameBuffer(target);
    }
    
    public void BeginFrame(Matrix viewProjectionMatrix) => BeginFrame(viewProjectionMatrix, m_graphicsDevice.swapChainFrameBuffer);

    public void EndFrame()
    {
        m_commandList.End();
        m_graphicsDevice.Submit(m_commandList);
    }

    public void Dispose()
    {
        m_commandList.Dispose();
        
        // Quad
        m_quadMesh.Dispose();
        m_quadMaterial.Dispose();
        m_quadResources.Dispose();
    }
}
