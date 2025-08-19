using InnoInternal.Render.Bridge;

namespace InnoInternal.Render.Impl;

public interface IGraphicsDevice : IDisposable
{
    IFrameBuffer swapChainFrameBuffer { get; }
    
    IVertexBuffer CreateVertexBuffer(uint sizeInBytes);
    IIndexBuffer CreateIndexBuffer(uint sizeInBytes);
    IUniformBuffer CreateUniformBuffer(uint sizeInBytes);
    
    IFrameBuffer CreateFrameBuffer(FrameBufferDescription desc);
    IResourceSet CreateResourceSet(ResourceSetDescription desc);
    IReadOnlyList<IShader> CreateVertexFragmentShaders(ShaderDescription vertexDesc, ShaderDescription fragmentDesc);
    IShader CreateComputeShader(ShaderDescription desc);
    ITexture CreateTexture(TextureDescription desc);
    IPipelineState CreatePipelineState(PipelineStateDescription desc);
    
    ICommandList CreateCommandList();
    
    void Submit(ICommandList commandList);
    void SwapBuffers();
}