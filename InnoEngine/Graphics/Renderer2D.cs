using InnoBase;
using InnoInternal.Render.Impl;

namespace InnoEngine.Graphics;

public static class Renderer2D
{
    private static IGraphicsDevice s_device = null!;
    private static ICommandList s_commandList = null!;
    private static IPipelineState s_pipeline = null!;
    
    private struct VertexPositionColor(Vector2 position, Color color)
    {
        public Vector2 position = position; // This is the position, in normalized device coordinates.
        public Color color = color;      // This is the color of the vertex.
    }

    public static void Init(IGraphicsDevice device)
    {
        s_device = device;
        s_commandList = device.CreateCommandList();
            
        // TODO: 创建默认 Pipeline (顶点 + fragment shader)
        s_pipeline = BuildDefaultPipeline();
    }

    public static void BeginScene()
    {
        s_commandList.Begin();
        s_commandList.SetFrameBuffer(s_device.swapChainFrameBuffer);
        s_commandList.ClearColor(Color.BLACK);
        s_commandList.SetPipelineState(s_pipeline);
    }

    public static void DrawRect(Rect rect, Color color)
    {
        VertexPositionColor[] vertices =
        {
            new(new Vector2(rect.left, rect.top), color),
            new(new Vector2(rect.right, rect.top), color),
            new(new Vector2(rect.left, rect.bottom), color),
            new(new Vector2(rect.right, rect.bottom), color)
        };

        ushort[] indices = { 0, 1, 2, 1, 3, 2 };

        IVertexBuffer vb = s_device.CreateVertexBuffer((uint)(vertices.Length * 24));
        vb.Update(vertices);

        IIndexBuffer ib = s_device.CreateIndexBuffer((uint)(indices.Length * sizeof(ushort)));
        ib.Update(indices);

        s_commandList.SetVertexBuffer(vb);
        s_commandList.SetIndexBuffer(ib);
        s_commandList.DrawIndexed(6, 0);

        vb.Dispose();
        ib.Dispose();
    }

    public static void EndScene()
    {
        s_commandList.End();
        s_device.Submit(s_commandList);
        s_device.SwapBuffers();
    }

    private static IPipelineState BuildDefaultPipeline()
    {
        // TODO: 构建顶点 + fragment shader pipeline
        return null!;
    }
}