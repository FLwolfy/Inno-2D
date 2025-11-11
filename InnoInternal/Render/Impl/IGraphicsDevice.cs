namespace InnoInternal.Render.Impl;

public interface IGraphicsDevice : IDisposable
{
    IFrameBuffer swapchainFrameBuffer { get; }
    
    IVertexBuffer CreateVertexBuffer(uint sizeInBytes);
    IIndexBuffer CreateIndexBuffer(uint sizeInBytes);
    IUniformBuffer CreateUniformBuffer<T>(string name) where T: unmanaged;
    
    IFrameBuffer CreateFrameBuffer(FrameBufferDescription desc);
    IResourceSet CreateResourceSet(ResourceSetBinding binding);
    (IShader, IShader) CreateVertexFragmentShader(ShaderDescription vertDesc, ShaderDescription fragDesc);
    IShader CreateComputeShader(ShaderDescription desc);
    
    ITexture CreateTexture(TextureDescription desc);
    IPipelineState CreatePipelineState(PipelineStateDescription desc);
    
    ICommandList CreateCommandList();
    
    void Submit(ICommandList commandList);
    void SwapBuffers();
}