using InnoBase;
using InnoInternal.Resource.Impl;

namespace InnoInternal.Render.Impl;

internal interface IRenderTarget : IDisposable
{
    uint width { get; }
    uint height { get; }
    ITexture2D GetColorTexture();
}