namespace InnoInternal.Render.Impl;

public struct PipelineStateDescription
{
    public IShader VertexShader;
    public IShader FragmentShader;
}

public interface IPipelineState : IDisposable;
