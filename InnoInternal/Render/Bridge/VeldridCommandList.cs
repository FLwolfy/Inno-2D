using InnoInternal.Render.Impl;
using Veldrid;

namespace InnoInternal.Render.Bridge;

internal class VeldridCommandList : ICommandList
{
    internal CommandList inner { get; }

    public VeldridCommandList(CommandList commandList)
    {
        inner = commandList;
    }

    public void Begin()
    {
        inner.Begin();
    }

    public void End()
    {
        inner.End();
    }

    public void SetFrameBuffer(IFrameBuffer frameBuffer)
    {
        if (frameBuffer is VeldridFrameBuffer veldridFB)
            inner.SetFramebuffer(veldridFB.Framebuffer);
    }

    public void SetVertexBuffer(IVertexBuffer vertexBuffer)
    {
        if (vertexBuffer is VeldridVertexBuffer veldridVB)
            inner.SetVertexBuffer(0, veldridVB.inner);
    }

    public void SetIndexBuffer(IIndexBuffer indexBuffer)
    {
        if (indexBuffer is VeldridIndexBuffer veldridIB)
            inner.SetIndexBuffer(veldridIB.inner, IndexFormat.UInt16); // or UInt32 based on your index data
    }

    public void SetResourceSet(int setIndex, IResourceSet resourceSet)
    {
        if (resourceSet is VeldridResourceSet veldridRS)
            inner.SetGraphicsResourceSet((uint)setIndex, veldridRS.ResourceSet);
    }

    public void SetPipelineState(IPipelineState pipelineState)
    {
        if (pipelineState is VeldridPipelineState veldridPS)
            inner.SetPipeline(veldridPS.Pipeline);
    }

    public void Draw(uint vertexCount, uint startVertex = 0)
    {
        inner.Draw(vertexCount, startVertex);
    }

    public void DrawIndexed(uint indexCount, uint startIndex = 0, int baseVertex = 0)
    {
        inner.DrawIndexed(indexCount, startIndex, baseVertex);
    }

    public void ClearColor(float r, float g, float b, float a)
    {
        inner.ClearColorTarget(0, RgbaFloat.FromBytes((byte)(r * 255), (byte)(g * 255), (byte)(b * 255), (byte)(a * 255)));
    }

    public void ClearDepth(float depth)
    {
        inner.ClearDepthStencil(depth);
    }

    public void Dispose()
    {
        inner.Dispose();
    }
}
