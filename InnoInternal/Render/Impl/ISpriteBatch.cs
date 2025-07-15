using InnoBase;
using InnoInternal.Resource.Impl;

namespace InnoInternal.Render.Impl;

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