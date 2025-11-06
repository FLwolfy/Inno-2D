using InnoBase;
using InnoInternal.Render.Impl;

using Veldrid;

namespace InnoInternal.Render.Bridge;

internal class VeldridCommandList : ICommandList
{
    private Framebuffer m_currentFrameBuffer;
    
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
        {
            inner.SetFramebuffer(veldridFB.inner);
            m_currentFrameBuffer = veldridFB.inner;
        }
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
            inner.SetGraphicsResourceSet((uint)setIndex, veldridRS.inner);
    }

    public void SetPipelineState(IPipelineState pipelineState)
    {
        if (pipelineState is VeldridPipelineState veldridPS)
        {
            veldridPS.SetFrameBufferOutputDescription(m_currentFrameBuffer.OutputDescription);
            inner.SetPipeline(veldridPS.inner);
        }
    }

    public void UpdateUniform<T>(IUniformBuffer uniformBuffer, ref T data) where T : unmanaged
    {
        if (uniformBuffer is VeldridUniformBuffer veldridUB)
        {
            inner.UpdateBuffer(veldridUB.inner, 0, data);
        }
    }

    public void Draw(uint vertexCount, uint startVertex = 0)
    {
        inner.Draw(vertexCount, 1, startVertex, 0);
    }

    public void DrawIndexed(uint indexCount, uint startIndex = 0, int baseVertex = 0)
    {
        inner.DrawIndexed(indexCount, 1, startIndex, baseVertex, 0);
    }

    public void ClearColor(Color color)
    {
        inner.ClearColorTarget(0, new RgbaFloat(color.r, color.g, color.b, color.a));
    }

    public void Dispose()
    {
        inner.Dispose();
    }
}
