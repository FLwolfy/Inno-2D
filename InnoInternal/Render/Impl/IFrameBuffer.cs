using InnoBase;

namespace InnoInternal.Render.Impl;

public struct FrameBufferDescription
{
    public ITexture? depthAttachment;
    public ITexture[] colorAttachments;
}


public interface IFrameBuffer : IDisposable
{
    int width { get; }
    int height { get; }

    ITexture GetAttachment(int index);
    ITexture? GetDepthAttachment();
}
