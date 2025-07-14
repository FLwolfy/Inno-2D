using InnoEngine.Internal.Base;
using InnoEngine.Internal.Resource.Impl;

namespace InnoEngine.Internal.Render.Impl;

internal interface ISpriteBatch
{
    void Begin();
    void DrawQuad(
        Rect destinationRect,
        Rect? sourceRect,
        ITexture2D texture,
        Color color,
        float rotation = 0f,
        float layerDepth = 0f,
        Vector2? origin = null
    );
    void End();
}