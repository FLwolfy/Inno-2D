namespace Inno.Platform.Graphics;

public enum PrimitiveTopology
{
    TriangleList,
    TriangleStrip,
    LineList,
    LineStrip
}

public enum DepthStencilState
{
    Disabled,
    DepthOnlyLessEqual,
    DepthOnlyGreaterEqual,
    DepthReadOnlyLessEqual,
    DepthReadOnlyGreaterEqual
}

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
