using InnoInternal.Render.Bridge;

namespace InnoInternal.Render.Impl;

public interface IGraphicsDevice : IDisposable
{
    IFrameBuffer swapChainFrameBuffer { get; }
    
    IVertexBuffer CreateVertexBuffer(uint sizeInBytes);
    IIndexBuffer CreateIndexBuffer(uint sizeInBytes);
    IUniformBuffer CreateUniformBuffer(uint sizeInBytes, string name);
    
    IFrameBuffer CreateFrameBuffer(FrameBufferDescription desc);
    IResourceSet CreateResourceSet(ResourceSetBinding binding);
    IShader CreateShader(ShaderDescription desc);
    ITexture CreateTexture(TextureDescription desc);
    IPipelineState CreatePipelineState(PipelineStateDescription desc);
    
    ICommandList CreateCommandList();
    
    void Submit(ICommandList commandList);
    void SwapBuffers();
}