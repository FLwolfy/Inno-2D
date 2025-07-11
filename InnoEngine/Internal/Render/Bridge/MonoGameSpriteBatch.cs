using InnoEngine.Internal.Base;
using InnoEngine.Internal.Render.Impl;

using Microsoft.Xna.Framework.Graphics;

namespace InnoEngine.Internal.Render.Bridge;

internal class MonoGameSpriteBatch : ISpriteBatch
{
    private readonly SpriteBatch m_spriteBatch;

    public MonoGameSpriteBatch(GraphicsDevice device)
    {
        m_spriteBatch = new SpriteBatch(device);
    }

    public void Begin()
    {
        // TODO: Handle parameters like sort mode, blend state, etc.
        //       Get rid of hardcoded values.
        m_spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
    }

    public void DrawQuad(Rect destinationRect, Rect? sourceRect, ITexture2D texture, Color color, float rotation = 0,
        float layerDepth = 0, Vector2? origin = null)
    {
        if (texture is not MonoGameTexture2D mgTexture)
            throw new ArgumentException("Texture must be MonoGameTexture2D", nameof(texture));

        var originVec = origin ?? new Vector2(0, 0);
        var xnaOrigin = new Microsoft.Xna.Framework.Vector2(originVec.x, originVec.y);
        var xnaColor = new Microsoft.Xna.Framework.Color(color.r, color.g, color.b, color.a);
        var xnaDestRect = new Microsoft.Xna.Framework.Rectangle(destinationRect.x, destinationRect.y, destinationRect.width, destinationRect.height);

        Microsoft.Xna.Framework.Rectangle? xnaSrcRect = null;
        if (sourceRect.HasValue)
        {
            var src = sourceRect.Value;
            xnaSrcRect = new Microsoft.Xna.Framework.Rectangle(src.x, src.y, src.width, src.height);
        }

        m_spriteBatch.Draw(
            mgTexture.rawTexture,
            xnaDestRect,
            xnaSrcRect,
            xnaColor,
            rotation,
            xnaOrigin,
            SpriteEffects.None,
            layerDepth
        );
    }


    public void End()
    {
        m_spriteBatch.End();
    }
}