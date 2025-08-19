using InnoInternal.Render.Impl;

namespace InnoInternal.Render.Bridge;

internal class VeldridTexture : ITexture
{
    public int width { get; }
    public int height { get; }
    
    public void Dispose()
    {
        throw new NotImplementedException();
    }
}