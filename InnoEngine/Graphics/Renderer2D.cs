using InnoBase;
using InnoInternal.Render.Impl;

namespace InnoEngine.Graphics;

public class Renderer2D : IDisposable
{
    private readonly IGraphicsDevice m_graphicsDevice;
    private ICommandList m_commandList = null!;
    private IVertexBuffer m_vertexBuffer = null!;
    private IIndexBuffer m_indexBuffer = null!;
    private IPipelineState m_pipeline = null!;
    private IShader m_vertexShader = null!;
    private IShader m_fragmentShader = null!;

    private struct VertexPositionColor(Vector2 position, Color color)
    {
        public Vector2 position = position;
        public Color color = color;
    }

    private const string VertexShaderCode = @"
#version 450
layout(location = 0) in vec2 Position;
layout(location = 1) in vec4 Color;

layout(location = 0) out vec4 fsin_Color;

void main()
{
    gl_Position = vec4(Position, 0, 1);
    fsin_Color = Color;
}";

    private const string FragmentShaderCode = @"
#version 450
layout(location = 0) in vec4 fsin_Color;
layout(location = 0) out vec4 fsout_Color;

void main()
{
    fsout_Color = fsin_Color;
}";

    public Renderer2D(IGraphicsDevice graphicsDevice)
    {
        m_graphicsDevice = graphicsDevice;
    }
    
    public void LoadResources()
    {
        CreateResources();
    }

    private void CreateResources()
    {
        VertexPositionColor[] quadVertices =
        {
            new VertexPositionColor(new Vector2(-0.5f, 0.5f), Color.WHITE),
            new VertexPositionColor(new Vector2(0.5f, 0.5f), Color.WHITE),
            new VertexPositionColor(new Vector2(-0.5f, -0.5f), Color.WHITE),
            new VertexPositionColor(new Vector2(0.5f, -0.5f), Color.WHITE)
        };

        ushort[] quadIndices = { 0, 1, 2, 1, 3, 2 };
        int vertexSize = sizeof(float) * 2 + sizeof(float) * 4;

        m_vertexBuffer = m_graphicsDevice.CreateVertexBuffer((uint)(quadVertices.Length * vertexSize));
        m_indexBuffer = m_graphicsDevice.CreateIndexBuffer((uint)(quadIndices.Length * sizeof(ushort)));

        m_vertexBuffer.Set(quadVertices);
        m_indexBuffer.Set(quadIndices);

        // Shader
        var vertexDesc = new ShaderDescription
        {
            stage = ShaderStage.Vertex,
            sourceCode = VertexShaderCode
        };
        var fragmentDesc = new ShaderDescription
        {
            stage = ShaderStage.Fragment,
            sourceCode = FragmentShaderCode
        };

        (m_vertexShader, m_fragmentShader) = m_graphicsDevice.CreateVertexFragmentShader(vertexDesc, fragmentDesc);

        // Pipeline
        var pipelineDesc = new PipelineStateDescription
        {
            vertexShader = m_vertexShader,
            fragmentShader = m_fragmentShader,
            vertexLayoutType = typeof(VertexPositionColor),
            resourceSetBindings = []
        };
        m_pipeline = m_graphicsDevice.CreatePipelineState(pipelineDesc);

        // CommandList
        m_commandList = m_graphicsDevice.CreateCommandList();
    }
    
    public void DrawQuad(Vector2 position, Vector2 size, Color color)
    {
        VertexPositionColor[] vertices =
        {
            new VertexPositionColor(position + new Vector2(-size.x, size.y) * 0.5f, color),
            new VertexPositionColor(position + new Vector2(size.x, size.y) * 0.5f, color),
            new VertexPositionColor(position + new Vector2(-size.x, -size.y) * 0.5f, color),
            new VertexPositionColor(position + new Vector2(size.x, -size.y) * 0.5f, color)
        };

        ushort[] indices = { 0, 1, 2, 1, 3, 2 };

        m_vertexBuffer.Set(vertices);
        m_indexBuffer.Set(indices);

        m_commandList.SetPipelineState(m_pipeline);
        m_commandList.SetVertexBuffer(m_vertexBuffer);
        m_commandList.SetIndexBuffer(m_indexBuffer);
        m_commandList.DrawIndexed(6, 0);
    }

    public void BeginFrame(IFrameBuffer target)
    {
        m_commandList.Begin();
        m_commandList.SetFrameBuffer(target);
        m_commandList.ClearColor(Color.BLACK); // TODO: Move to Clear
    }
    
    public void BeginFrame()
    {
        BeginFrame(m_graphicsDevice.swapChainFrameBuffer);
    }

    public void EndFrame()
    {
        m_commandList.End();
        m_graphicsDevice.Submit(m_commandList);
    }

    public void Dispose()
    {
        m_pipeline.Dispose();
        m_commandList.Dispose();
        m_vertexBuffer.Dispose();
        m_indexBuffer.Dispose();
        m_vertexShader.Dispose();
        m_fragmentShader.Dispose();
    }
}
