namespace InnoInternal.Render.Impl;

public struct FrameBufferDescription
{
    public int width;
    public int height;
    public TextureDescription? depthAttachmentDescription;
    public TextureDescription[] colorAttachmentDescriptions;
}

public interface IFrameBuffer : IDisposable
{
    int width { get; }
    int height { get; }
    
    void Resize(int width, int height);

    ITexture? GetColorAttachment(int index);
    ITexture? GetDepthAttachment();
}
