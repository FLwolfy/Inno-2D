using InnoInternal.Render.Impl;

namespace InnoInternal.Render.Bridge;

public class VeldridShader : IShader
{
    public ShaderStage stage { get; }
    
    public void Dispose()
    {
        throw new NotImplementedException();
    }

}