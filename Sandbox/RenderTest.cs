using InnoBase;
using InnoInternal.Render.Bridge;
using InnoInternal.Render.Impl;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;
using ShaderDescription = InnoInternal.Render.Impl.ShaderDescription;

namespace Sandbox;

internal class RenderTest
{
    private readonly Sdl2Window m_window;
    private readonly IGraphicsDevice m_graphicsDevice;
    
    private ICommandList m_commandList = null!;
    private IVertexBuffer m_vertexBuffer = null!;
    private IIndexBuffer m_indexBuffer = null!;
    private IUniformBuffer m_transformBuffer = null!;
    private IPipelineState m_pipeline = null!;
    private IResourceSet m_resourceSet = null!;
    
    private TransformBuffer m_transform;
    private float m_rotationAngle;
    
    private const string C_VERTEX_CODE = @"
#include <metal_stdlib>
using namespace metal;

// === 与 GLSL layout(std140) uniform 对应 ===
struct TransformBuffer
{
    float4x4 uPosition;
    float4x4 uRotation;
    float4x4 uScale;
};

// === 顶点输入结构，对应 GLSL 的 in ===
struct VertexInput
{
    float2 position [[attribute(0)]];
    float4 color    [[attribute(1)]];
};

// === 顶点输出结构，对应 GLSL 的 out ===
struct VertexOutput
{
    float4 position [[position]];
    float4 color;
};

// === 顶点主函数 ===
vertex VertexOutput main_vertex(
    VertexInput in                [[stage_in]],
    constant TransformBuffer& transform [[buffer(1)]]
)
{
    VertexOutput out;
    float4 worldPos = float4(in.position, 0.0, 1.0);
    float4x4 model = transform.uPosition * transform.uRotation * transform.uScale;
    out.position = model * worldPos;
    out.color = in.color;
    return out;
}
";

    private const string C_FRAGMENT_CODE = @"
#include <metal_stdlib>
using namespace metal;

// === 片段输入结构 ===
struct VertexOutput
{
    float4 position [[position]];
    float4 color;
};

// === 主函数 ===
fragment float4 main_fragment(VertexOutput in [[stage_in]])
{
    return in.color;
}

";
    
    private struct VertexPositionColor(Vector2 position, Color color)
    {
        public Vector2 position = position;
        public Color color = color;
    }
    
    private struct TransformBuffer(Matrix uPosition, Matrix uRotation, Matrix uScale)
    {
        public Matrix uPosition = uPosition;
        public Matrix uRotation = uRotation;
        public Matrix uScale = uScale;
    }
    
    public RenderTest()
    { 
        WindowCreateInfo windowCi = new WindowCreateInfo()
        {
            X = 100,
            Y = 100,
            WindowWidth = 600,
            WindowHeight = 600,
            WindowTitle = "Render Example"
        }; 
        m_window = VeldridStartup.CreateWindow(ref windowCi);
        m_graphicsDevice = new VeldridGraphicsDevice(VeldridStartup.CreateGraphicsDevice(m_window, GraphicsBackend.Metal));
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
        
        // Rotation test
        m_rotationAngle += 0.0010f; 
        m_transform.uRotation = Matrix.CreateRotationZ(m_rotationAngle);

        m_commandList.UpdateUniform(m_transformBuffer, ref m_transform);
        
        m_commandList.SetFrameBuffer(m_graphicsDevice.swapChainFrameBuffer);
        m_commandList.ClearColor(Color.BLACK);
        m_commandList.SetPipelineState(m_pipeline);
        m_commandList.SetVertexBuffer(m_vertexBuffer);
        m_commandList.SetIndexBuffer(m_indexBuffer);
        m_commandList.SetResourceSet(0, m_resourceSet);
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

        m_transform = new(
            Matrix.CreateTranslation(new Vector3(0, 0, 0)),
            Matrix.CreateRotationZ(45),
            Matrix.CreateScale(0.5f)
        );

        uint vertexSize = (uint)(4 * sizeof(VertexPositionColor));
        uint indexSize = 6 * sizeof(ushort);

        m_vertexBuffer = m_graphicsDevice.CreateVertexBuffer(vertexSize);
        m_indexBuffer = m_graphicsDevice.CreateIndexBuffer(indexSize);
        m_transformBuffer = m_graphicsDevice.CreateUniformBuffer<TransformBuffer>("Transform");

        m_vertexBuffer.Set(quadVertices);
        m_indexBuffer.Set(quadIndices);
        
        ShaderDescription vertexShaderDesc = new ShaderDescription();
        vertexShaderDesc.stage = ShaderStage.Vertex;
        vertexShaderDesc.entryPoint = "main_vertex";
        vertexShaderDesc.sourceCode = C_VERTEX_CODE;
            
        ShaderDescription fragmentShaderDesc = new ShaderDescription();
        fragmentShaderDesc.stage = ShaderStage.Fragment;
        fragmentShaderDesc.entryPoint = "main_fragment";
        fragmentShaderDesc.sourceCode = C_FRAGMENT_CODE;
            
        IShader vertexShader = m_graphicsDevice.CreateShader(vertexShaderDesc);
        IShader fragmentShader = m_graphicsDevice.CreateShader(fragmentShaderDesc);
        
        ResourceSetBinding resourceSetBinding = new ResourceSetBinding
        {
            shaderStages = ShaderStage.Vertex,
            uniformBuffers = [m_transformBuffer]
        };
        
        PipelineStateDescription pipelineDesc = new PipelineStateDescription
        {
            vertexShader = vertexShader,
            fragmentShader = fragmentShader,
            vertexLayoutType = typeof(VertexPositionColor),
            resourceSetBindings = [resourceSetBinding]
        };

        m_resourceSet = m_graphicsDevice.CreateResourceSet(resourceSetBinding);
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