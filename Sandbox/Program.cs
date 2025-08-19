using System.Runtime.InteropServices;
using InnoBase;
using InnoInternal.Render.Bridge;
using InnoInternal.Render.Impl;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;

namespace GettingStarted
{
    class Program
    {
        private static IGraphicsDevice _graphicsDevice;
        private static ICommandList _commandList;
        private static IVertexBuffer _vertexBuffer;
        private static IIndexBuffer _indexBuffer;
        private static IPipelineState _pipeline;

        private const string VertexCode = @"
#version 450

layout(location = 0) in vec2 Position;
layout(location = 1) in vec4 Color;

layout(location = 0) out vec4 fsin_Color;

void main()
{
    gl_Position = vec4(Position, 0, 1);
    fsin_Color = Color;
}";

        private const string FragmentCode = @"
#version 450

layout(location = 0) in vec4 fsin_Color;
layout(location = 0) out vec4 fsout_Color;

void main()
{
    fsout_Color = fsin_Color;
}";

        static void Main()
        {
            WindowCreateInfo windowCI = new WindowCreateInfo()
            {
                X = 100,
                Y = 100,
                WindowWidth = 960,
                WindowHeight = 540,
                WindowTitle = "Veldrid Tutorial"
            };
            Sdl2Window window = VeldridStartup.CreateWindow(ref windowCI);

            _graphicsDevice = new VeldridGraphicsDevice(VeldridStartup.CreateGraphicsDevice(window));

            CreateResources();

            while (window.Exists)
            {
                window.PumpEvents();
                Draw();
            }
            
            DisposeResources();
        }
        
        private static void Draw()
        {
            _commandList.Begin();
            _commandList.SetFrameBuffer(_graphicsDevice.swapChainFrameBuffer);
            _commandList.ClearColor(Color.BLACK);
            _commandList.SetPipelineState(_pipeline);
            _commandList.SetVertexBuffer(_vertexBuffer);
            _commandList.SetIndexBuffer(_indexBuffer);
            _commandList.DrawIndexed(6, 0);
            _commandList.End();

            _graphicsDevice.Submit(_commandList);
            _graphicsDevice.SwapBuffers();
        }

        private static void CreateResources()
        {
            VertexPositionColor[] quadVertices =
            {
                new(new Vector2(-0.75f, 0.75f), Color.RED),
                new(new Vector2(0.75f, 0.75f), Color.GREEN),
                new(new Vector2(-0.75f, -0.75f), Color.BLUE),
                new(new Vector2(0.75f, -0.75f), Color.YELLOW)
            };
            ushort[] quadIndices = { 0, 1, 2, 1, 3, 2 };
            uint vertexSize = (uint)(4 * Marshal.SizeOf<VertexPositionColor>());
            uint indexSize = 6 * sizeof(ushort);

            _vertexBuffer = _graphicsDevice.CreateVertexBuffer(vertexSize);
            _indexBuffer = _graphicsDevice.CreateIndexBuffer(indexSize);

            _vertexBuffer.Update(quadVertices);
            _indexBuffer.Update(quadIndices);

            ShaderDescription vertexShaderDesc = new ShaderDescription();
            vertexShaderDesc.stage = ShaderStage.Vertex;
            vertexShaderDesc.entryPoint = "main";
            vertexShaderDesc.sourceCode = VertexCode;
            
            ShaderDescription fragmentShaderDesc = new ShaderDescription();
            fragmentShaderDesc.stage = ShaderStage.Fragment;
            fragmentShaderDesc.entryPoint = "main";
            fragmentShaderDesc.sourceCode = FragmentCode;
            
            var shaders = _graphicsDevice.CreateVertexFragmentShaders(vertexShaderDesc, fragmentShaderDesc);
            IShader vertexShader = shaders[0];
            IShader fragmentShader = shaders[1];

            PipelineStateDescription pipelineDesc = new PipelineStateDescription();
            pipelineDesc.vertexShader = vertexShader;
            pipelineDesc.fragmentShader = fragmentShader;
            pipelineDesc.vertexLayoutType = typeof(VertexPositionColor);
            
            _pipeline = _graphicsDevice.CreatePipelineState(pipelineDesc);

            _commandList = _graphicsDevice.CreateCommandList();
        }
        
        private static void DisposeResources()
        {
            _pipeline.Dispose();
            _commandList.Dispose();
            _vertexBuffer.Dispose();
            _indexBuffer.Dispose();
            _graphicsDevice.Dispose();
        }
    }

    struct VertexPositionColor(Vector2 position, Color color)
    {
        public Vector2 position = position; // This is the position, in normalized device coordinates.
        public Color color = color;      // This is the color of the vertex.
    }
}