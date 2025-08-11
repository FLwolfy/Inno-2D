using InnoBase;
using InnoInternal.Render.Impl;

namespace InnoInternal.Render.Bridge;

public class VeldridFrameBuffer : IFrameBuffer
{
    public int width { get; }
    public int height { get; }
    
    public void Clear(Color color)
    {
        throw new NotImplementedException();
    }
    
    public void Dispose()
    {
        // TODO release managed resources here
    }
}