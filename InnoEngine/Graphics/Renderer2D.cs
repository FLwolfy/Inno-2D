using InnoBase;
using InnoEngine.ECS;
using InnoEngine.Graphics.RenderObject;
using InnoInternal.Render.Impl;

namespace InnoEngine.Graphics;

public class Renderer2D : IDisposable
{
    private readonly IGraphicsDevice m_graphicsDevice;
    private readonly ICommandList m_commandList;
    
    // Render Info 
    private IFrameBuffer m_sceneFrameBuffer;
    private IFrameBuffer m_currentFrameBuffer;
    private Matrix m_currentViewProjection;

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
            depthStencilState = DepthStencilState.DepthOnlyLessEqual,
            resourceLayoutSpecifiers = [resourceSetBinding]
        };
        var pipeline = m_graphicsDevice.CreatePipelineState(pipelineDesc);

        // Mesh and Material
        m_quadMesh = new Mesh(vb, ib);
        m_quadMaterial = new Material(vertexShader, fragmentShader, pipeline, [mvpBuffer, colorBuffer]);
    }
    
    public void DrawQuad(Matrix transform, Color color)
    {
        var mvp = transform * m_currentViewProjection;
        
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

    // TODO: Make target not nullable
    public void BeginFrame(Matrix viewProjectionMatrix, float? aspectRatio, IFrameBuffer? target)
    {
        m_currentFrameBuffer = target ?? m_graphicsDevice.swapchainFrameBuffer;
        m_currentViewProjection = viewProjectionMatrix;
        
        m_commandList.Begin();
        m_commandList.SetFrameBuffer(m_currentFrameBuffer);
        m_commandList.ClearColor(Color.BLACK);

        // TODO: Move this into Graphics Device
        if (aspectRatio.HasValue)
        {
            float targetWidth = m_currentFrameBuffer.width;
            float targetHeight = m_currentFrameBuffer.height;

            float screenAspect = targetWidth / targetHeight;
            float sourceAspect = aspectRatio.Value;

            Rect viewportRect;

            // Left Right
            if (screenAspect > sourceAspect)
            {
                float newWidth = targetHeight * sourceAspect;
                float xOffset = (targetWidth - newWidth) / 2f;
                viewportRect = new Rect((int)xOffset, 0, (int)newWidth, (int)targetHeight);
            }
        
            // Top Bottom
            else
            {
                float newHeight = targetWidth / sourceAspect;
                float yOffset = (targetHeight - newHeight) / 2f;
                viewportRect = new Rect(0, (int)yOffset, (int)targetWidth, (int)newHeight);
            }

            m_commandList.SetViewPort(0, viewportRect);
            m_commandList.SetScissorRect(0, viewportRect);
        }

        if (m_currentFrameBuffer.GetDepthAttachment() != null)
        {
            m_commandList.ClearDepth(1.0f);
        }
    }

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
