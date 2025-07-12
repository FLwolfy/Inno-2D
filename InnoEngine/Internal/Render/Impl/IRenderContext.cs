using InnoEngine.Internal.Base;

namespace InnoEngine.Internal.Render.Impl;

internal interface IRenderContext
{
    void BeginFrame();
    void EndFrame();
}