namespace InnoInternal.Render.Impl;

public struct PipelineStateDescription
{
    public IShader vertexShader;
    public IShader fragmentShader;
}

public interface IPipelineState : IDisposable;
