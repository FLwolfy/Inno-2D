using InnoBase;

namespace InnoInternal.Render.Impl;

public struct FrameBufferDescription
{
    public int width;
    public int height;
}

public interface IFrameBuffer : IDisposable
{
    int width { get; }
    int height { get; }
    
    void Clear(Color color);
}