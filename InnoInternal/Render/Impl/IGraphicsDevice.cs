namespace InnoInternal.Render.Impl;

public interface IGraphicsDevice : IDisposable
{
    IVertexBuffer CreateVertexBuffer<T>(T[] vertices, int sizeInBytes) where T : struct;
    IIndexBuffer CreateIndexBuffer<T>(T[] indices, int sizeInBytes) where T : struct;
    IUniformBuffer CreateUniformBuffer(int sizeInBytes);
    IFrameBuffer CreateFrameBuffer(FrameBufferDescription desc);
    IShader CreateShader(string shaderCode, ShaderStage stage);
    ITexture CreateTexture(int width, int height, byte[] pixelData);
    IPipelineState CreatePipelineState(PipelineStateDescription desc);
    ICommandList CreateCommandList();
    
    void Submit(ICommandList commandList);
}