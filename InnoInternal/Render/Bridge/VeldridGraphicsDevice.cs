using InnoInternal.Render.Impl;

using Veldrid;

namespace InnoInternal.Render.Bridge;

internal class VeldridGraphicsDevice : IGraphicsDevice
{
    private readonly GraphicsDevice m_graphicsDevice;
    private readonly ResourceFactory m_factory;

    public VeldridGraphicsDevice(GraphicsDevice graphicsDevice)
    {
        m_graphicsDevice = graphicsDevice;
        m_factory = graphicsDevice.ResourceFactory;
    }

    public IVertexBuffer CreateVertexBuffer<T>(T[] vertices, int sizeInBytes) where T : unmanaged
    {
        var vb = m_factory.CreateBuffer(new BufferDescription((uint)sizeInBytes, BufferUsage.VertexBuffer));
        return new VeldridVertexBuffer(m_graphicsDevice, vb);
    }

    public IIndexBuffer CreateIndexBuffer<T>(T[] indices, int sizeInBytes) where T : unmanaged
    {
        var ib = m_factory.CreateBuffer(new BufferDescription((uint)sizeInBytes, BufferUsage.IndexBuffer));
        return new VeldridIndexBuffer(m_graphicsDevice, ib);
    }

    public IUniformBuffer CreateUniformBuffer(int sizeInBytes)
    {
        var ub = m_factory.CreateBuffer(new BufferDescription((uint)sizeInBytes, BufferUsage.UniformBuffer | BufferUsage.Dynamic));
        return new VeldridUniformBuffer(ub);
    }

    public IFrameBuffer CreateFrameBuffer(FrameBufferDescription desc)
    {
        throw new NotImplementedException();
    }

    public IShader CreateShader(string shaderCode, ShaderStage stage)
    {
        throw new NotImplementedException();
    }

    public ITexture CreateTexture(int width, int height, byte[] pixelData)
    {
        throw new NotImplementedException();
    }

    public IPipelineState CreatePipelineState(PipelineStateDescription desc)
    {
        throw new NotImplementedException();
    }

    public ICommandList CreateCommandList()
    {
        var cmdList = m_factory.CreateCommandList();
        return new VeldridCommandList(cmdList);
    }

    public void Submit(ICommandList commandList)
    {
        if (commandList is VeldridCommandList veldridCmd)
        {
            m_graphicsDevice.SubmitCommands(veldridCmd.inner);
            m_graphicsDevice.WaitForIdle();  
        }
    }

    public void Dispose()
    {
        m_graphicsDevice.Dispose();
    }
}
