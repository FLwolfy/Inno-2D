using InnoBase.Graphics;

namespace InnoInternal.Render.Impl;

public struct PipelineStateDescription
{
    public IShader vertexShader;
    public IShader fragmentShader;
    public List<Type> vertexLayoutTypes;
    
    public PrimitiveTopology primitiveTopology;
    public DepthStencilState depthStencilState;
    
    public ResourceSetBinding[]? resourceLayoutSpecifiers;
}

public interface IPipelineState : IDisposable;
