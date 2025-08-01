using InnoInternal.Resource.Impl;

namespace InnoInternal.Render.Impl;

internal interface IRenderTarget : IDisposable
{
    int width { get; }
    int height { get; }
    ITexture2D GetColorTexture();
}