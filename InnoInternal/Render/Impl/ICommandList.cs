namespace InnoInternal.Render.Impl;

public interface ICommandList : IDisposable
{
    void Begin();
    void End();

    void SetPipelineState(IPipelineState pipelineState);
    void SetVertexBuffer(int slot, IVertexBuffer vertexBuffer);
    void SetIndexBuffer(IIndexBuffer indexBuffer);

    void Draw(uint vertexCount, uint startVertex = 0);
    void DrawIndexed(uint indexCount, uint startIndex = 0, int baseVertex = 0);

    void ClearColor(float r, float g, float b, float a);
    void ClearDepth(float depth);
}