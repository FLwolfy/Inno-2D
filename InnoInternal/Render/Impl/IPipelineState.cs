namespace InnoInternal.Render.Impl;

public struct PipelineStateDescription
{
    public IShader vertexShader;
    public IShader fragmentShader;
    public Type vertexLayoutType;
}

public interface IPipelineState : IDisposable;
