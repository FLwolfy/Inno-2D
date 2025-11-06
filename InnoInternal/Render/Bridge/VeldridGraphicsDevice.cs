using InnoInternal.Render.Impl;

using Veldrid;

using InnoFBDescription = InnoInternal.Render.Impl.FrameBufferDescription;
using ShaderDescription = InnoInternal.Render.Impl.ShaderDescription;
using TextureDescription = InnoInternal.Render.Impl.TextureDescription;

using VeldridFBDescription = Veldrid.FramebufferDescription;

namespace InnoInternal.Render.Bridge;

public class VeldridGraphicsDevice : IGraphicsDevice
{
    private readonly GraphicsDevice m_graphicsDevice;
    private readonly ResourceFactory m_factory;
    
    internal GraphicsDevice inner => m_graphicsDevice;
    public IFrameBuffer swapChainFrameBuffer { get; }

    public VeldridGraphicsDevice(GraphicsDevice graphicsDevice)
    {
        m_graphicsDevice = graphicsDevice;
        
        swapChainFrameBuffer = new VeldridFrameBuffer(graphicsDevice, graphicsDevice.SwapchainFramebuffer);
        m_factory = graphicsDevice.ResourceFactory;
    }

    public IVertexBuffer CreateVertexBuffer(uint sizeInBytes)
    {
        var vb = m_factory.CreateBuffer(new BufferDescription(sizeInBytes, BufferUsage.VertexBuffer));
        return new VeldridVertexBuffer(m_graphicsDevice, vb);
    }

    public IIndexBuffer CreateIndexBuffer(uint sizeInBytes)
    {
        var ib = m_factory.CreateBuffer(new BufferDescription(sizeInBytes, BufferUsage.IndexBuffer));
        return new VeldridIndexBuffer(m_graphicsDevice, ib);
    }

    public IUniformBuffer CreateUniformBuffer(uint sizeInBytes, String name)
    {
        var ub = m_factory.CreateBuffer(new BufferDescription(sizeInBytes, BufferUsage.UniformBuffer | BufferUsage.Dynamic));
        return new VeldridUniformBuffer(m_graphicsDevice, ub, name);
    }

    public IFrameBuffer CreateFrameBuffer(InnoFBDescription desc)
    {
        throw new NotImplementedException();
    }

    public IResourceSet CreateResourceSet(ResourceSetBinding binding)
    {
        return new VeldridResourceSet(m_graphicsDevice, binding);
    }
    
    public IShader CreateShader(ShaderDescription desc)
    {
        return VeldridShader.CreateShader(m_graphicsDevice, desc);
    }

    public ITexture CreateTexture(TextureDescription desc)
    {
        throw new NotImplementedException();
    }

    public IPipelineState CreatePipelineState(PipelineStateDescription desc)
    {
        return new VeldridPipelineState(m_graphicsDevice, desc);
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

    public void SwapBuffers()
    {
        m_graphicsDevice.SwapBuffers();
    }

    public void Dispose()
    {
        m_graphicsDevice.Dispose();
    }
}
