using InnoBase;

namespace InnoInternal.Render.Impl;

public struct FrameBufferDescription
{
    public int width;
    public int height;
    public int attachmentCount;
    public bool hasDepth;
}


public interface IFrameBuffer : IDisposable
{
    int width { get; }
    int height { get; }

    ITexture GetAttachment(int index);
    ITexture? GetDepthAttachment();
    
    void Clear(Color clearColor);
    void Resize(int width, int height);
}
