namespace InnoInternal.Render.Impl;

public interface IGraphicsDevice : IDisposable
{
    IVertexBuffer CreateVertexBuffer<T>(T[] vertices) where T : struct;
    IIndexBuffer CreateIndexBuffer<T>(T[] indices) where T : struct;
    IShader CreateShader(string shaderCode, ShaderStage stage);
    ITexture CreateTexture(int width, int height, byte[] pixelData);
    IPipelineState CreatePipelineState(PipelineStateDescription desc);
    ICommandList CreateCommandList();
    
    void Submit(ICommandList commandList);
}