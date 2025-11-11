using InnoBase.Graphics;

namespace InnoInternal.Render.Impl;

public struct PipelineStateDescription
{
    public IShader vertexShader;
    public IShader fragmentShader;
    public Type vertexLayoutType;
    
    public DepthStencilState depthStencilState;
    public ResourceSetBinding[]? resourceLayoutSpecifiers;
}

public interface IPipelineState : IDisposable;
