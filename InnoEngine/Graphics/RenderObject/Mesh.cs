using InnoInternal.Render.Impl;

namespace InnoEngine.Graphics.RenderObject;

public class Mesh : IDisposable
{
    public IVertexBuffer vertexBuffer { get; }
    public IIndexBuffer indexBuffer { get; }

    public Mesh(IVertexBuffer vertexBuffer, IIndexBuffer indexBuffer)
    {
        this.vertexBuffer = vertexBuffer;
        this.indexBuffer = indexBuffer;
    }

    public void Dispose()
    {
        vertexBuffer.Dispose();
        indexBuffer.Dispose();
    }
}
