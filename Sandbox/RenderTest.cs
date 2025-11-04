using InnoBase;
using InnoInternal.Render.Bridge;
using InnoInternal.Render.Impl;

using Veldrid.Sdl2;
using Veldrid.StartupUtilities;

namespace Sandbox;

internal class RenderTest
{
    private readonly Sdl2Window m_window;
    private readonly IGraphicsDevice m_graphicsDevice;
    
    private ICommandList m_commandList = null!;
    private IVertexBuffer m_vertexBuffer = null!;
    private IIndexBuffer m_indexBuffer = null!;
    private IPipelineState m_pipeline = null!;

    private const string C_VERTEX_CODE = @"
#version 450

layout(location = 0) in vec2 Position;
layout(location = 1) in vec4 Color;

layout(location = 0) out vec4 fsin_Color;

void main()
{
    gl_Position = vec4(Position, 0, 1);
    fsin_Color = Color;
}";

    private const string C_FRAGMENT_CODE = @"
#version 450

layout(location = 0) in vec4 fsin_Color;
layout(location = 0) out vec4 fsout_Color;

void main()
{
    fsout_Color = fsin_Color;
}";
    
    private struct VertexPositionColor(Vector2 position, Color color)
    {
        public Vector2 position = position; // This is the position, in normalized device coordinates.
        public Color color = color;      // This is the color of the vertex.
    }

    public RenderTest()
    { 
        WindowCreateInfo windowCi = new WindowCreateInfo()
        {
            X = 100,
            Y = 100,
            WindowWidth = 960,
            WindowHeight = 540,
            WindowTitle = "Render Example"
        }; 
        m_window = VeldridStartup.CreateWindow(ref windowCi);
        m_graphicsDevice = new VeldridGraphicsDevice(VeldridStartup.CreateGraphicsDevice(m_window));
    }

    public void Run()
    {
        CreateResources();
        
        while (m_window.Exists)
        {
            m_window.PumpEvents();
            Draw();
        }
            
        DisposeResources();
    }
        
    private void Draw()
    {
        m_commandList.Begin();
        m_commandList.SetFrameBuffer(m_graphicsDevice.swapChainFrameBuffer);
        m_commandList.ClearColor(Color.BLACK);
        m_commandList.SetPipelineState(m_pipeline);
        m_commandList.SetVertexBuffer(m_vertexBuffer);
        m_commandList.SetIndexBuffer(m_indexBuffer);
        m_commandList.DrawIndexed(6, 0);
        m_commandList.End();

        m_graphicsDevice.Submit(m_commandList);
        m_graphicsDevice.SwapBuffers();
    }

    private unsafe void CreateResources()
    {
        VertexPositionColor[] quadVertices =
        {
            new(new Vector2(-0.75f, 0.75f), Color.RED),
            new(new Vector2(0.75f, 0.75f), Color.GREEN),
            new(new Vector2(-0.75f, -0.75f), Color.BLUE),
            new(new Vector2(0.75f, -0.75f), Color.YELLOW)
        };
        ushort[] quadIndices = { 0, 1, 2, 1, 3, 2 };
        uint vertexSize = (uint)(4 * sizeof(VertexPositionColor));
        uint indexSize = 6 * sizeof(ushort);

        m_vertexBuffer = m_graphicsDevice.CreateVertexBuffer(vertexSize);
        m_indexBuffer = m_graphicsDevice.CreateIndexBuffer(indexSize);

        m_vertexBuffer.Update(quadVertices);
        m_indexBuffer.Update(quadIndices);

        ShaderDescription vertexShaderDesc = new ShaderDescription();
        vertexShaderDesc.stage = ShaderStage.Vertex;
        vertexShaderDesc.entryPoint = "main";
        vertexShaderDesc.sourceCode = C_VERTEX_CODE;
            
        ShaderDescription fragmentShaderDesc = new ShaderDescription();
        fragmentShaderDesc.stage = ShaderStage.Fragment;
        fragmentShaderDesc.entryPoint = "main";
        fragmentShaderDesc.sourceCode = C_FRAGMENT_CODE;
            
        var shaders = m_graphicsDevice.CreateVertexFragmentShaders(vertexShaderDesc, fragmentShaderDesc);
        IShader vertexShader = shaders[0];
        IShader fragmentShader = shaders[1];

        PipelineStateDescription pipelineDesc = new PipelineStateDescription();
        pipelineDesc.vertexShader = vertexShader;
        pipelineDesc.fragmentShader = fragmentShader;
        pipelineDesc.vertexLayoutType = typeof(VertexPositionColor);
            
        m_pipeline = m_graphicsDevice.CreatePipelineState(pipelineDesc);

        m_commandList = m_graphicsDevice.CreateCommandList();
    }
        
    private void DisposeResources()
    {
        m_pipeline.Dispose();
        m_commandList.Dispose();
        m_vertexBuffer.Dispose();
        m_indexBuffer.Dispose();
        m_graphicsDevice.Dispose();
    }
}