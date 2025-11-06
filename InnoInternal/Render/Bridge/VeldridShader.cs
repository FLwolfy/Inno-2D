using System.Text;
using InnoInternal.Render.Impl;
using Veldrid;

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
    
    public static VeldridShader CreateShader(GraphicsDevice graphicsDevice, InnoSDescription desc)
    {
        var shader = graphicsDevice.ResourceFactory.CreateShader(ToVeldridSDesc(desc));
        return new VeldridShader(graphicsDevice, shader, desc.stage);
    }
    
    private static VeldridSDescription ToVeldridSDesc(InnoSDescription desc)
    {
        return new VeldridSDescription(
            ToVeldridShaderStage(desc.stage),
            Encoding.UTF8.GetBytes(desc.sourceCode),
            desc.entryPoint
        );
    }
    
    internal static VeldridShaderStage ToVeldridShaderStage(InnoShaderStage stage)
    {
        return (VeldridShaderStage)(byte)stage;
    }

}