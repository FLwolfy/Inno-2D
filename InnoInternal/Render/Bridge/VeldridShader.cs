using System.Text;
using InnoInternal.Render.Impl;
using Veldrid;
using Veldrid.SPIRV;

using InnoShaderStage = InnoInternal.Render.Impl.ShaderStage;
using InnoSDescription = InnoInternal.Render.Impl.ShaderDescription;
using VeldridShaderStage = Veldrid.ShaderStages;
using VeldridSDescription = Veldrid.ShaderDescription;

namespace InnoInternal.Render.Bridge;

internal class VeldridShader : IShader
{
    private readonly GraphicsDevice m_graphicsDevice;
    
    internal Shader inner { get; }
    public ShaderStage stage { get; }
    
    
    public VeldridShader(GraphicsDevice graphicsDevice, Shader inner, ShaderStage stage)
    {
        m_graphicsDevice = graphicsDevice;
        
        this.inner = inner;
        this.stage = stage;
    }
    
    public void Dispose()
    {
        throw new NotImplementedException();
    }
    
    public static VeldridShader[] CreateVertexFragment(GraphicsDevice graphicsDevice, InnoSDescription vertexDesc, InnoSDescription fragmentDesc)
    {
        var vertexFragmentShaders = graphicsDevice.ResourceFactory.CreateFromSpirv(ToVeldridSDesc(vertexDesc), ToVeldridSDesc(fragmentDesc));
        
        var vertexShader = new VeldridShader(graphicsDevice, vertexFragmentShaders[0], InnoShaderStage.Vertex);
        var fragmentShader = new VeldridShader(graphicsDevice, vertexFragmentShaders[1], InnoShaderStage.Fragment);
        
        return [vertexShader, fragmentShader];
    }
    
    public static VeldridShader CreateCompute(GraphicsDevice graphicsDevice, InnoSDescription desc)
    {
        var computeShader = graphicsDevice.ResourceFactory.CreateFromSpirv(ToVeldridSDesc(desc));
        return new VeldridShader(graphicsDevice, computeShader, desc.stage);
    }
    
    private static VeldridSDescription ToVeldridSDesc(InnoSDescription desc)
    {
        return new VeldridSDescription(
            ToVeldridShaderStage(desc.stage),
            Encoding.UTF8.GetBytes(desc.sourceCode),
            desc.entryPoint
        );
    }
    
    private static VeldridShaderStage ToVeldridShaderStage(InnoShaderStage shaderStage)
    {
        return shaderStage switch
        {
            InnoShaderStage.Vertex => VeldridShaderStage.Vertex,
            InnoShaderStage.Fragment => VeldridShaderStage.Fragment,
            InnoShaderStage.Compute => VeldridShaderStage.Compute,
            _ => throw new ArgumentOutOfRangeException(nameof(shaderStage), shaderStage, null)
        };
    }
}