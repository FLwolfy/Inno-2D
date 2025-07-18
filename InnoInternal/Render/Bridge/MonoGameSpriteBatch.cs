using InnoBase;
using InnoInternal.Render.Impl;
using InnoInternal.Resource.Bridge;
using InnoInternal.Resource.Impl;
using Microsoft.Xna.Framework.Graphics;

namespace InnoInternal.Render.Bridge;

internal class MonoGameSpriteBatch : ISpriteBatch
{
    private readonly SpriteBatch m_spriteBatch;
    private readonly IRenderContext m_renderContext;
    
    private static GraphicsDevice device => MonoGameRenderAPI.graphicsDevice;

    public MonoGameSpriteBatch(IRenderContext renderContext)
    {
        m_spriteBatch = new SpriteBatch(device);
        m_renderContext = renderContext;
    }

    public void Begin()
    {
        // TODO: Handle parameters like sort mode, blend state, etc. to get rid of hardcoded values.
        m_spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend,  transformMatrix: ToXnaMatrix(Matrix.Extract2DTransform(m_renderContext.worldToScreenMatrix)));
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

    private Microsoft.Xna.Framework.Matrix ToXnaMatrix(Matrix matrix)
    {
        return new Microsoft.Xna.Framework.Matrix(
            matrix.m11, matrix.m12, matrix.m13, matrix.m14,
            matrix.m21, matrix.m22, matrix.m23, matrix.m24,
            matrix.m31, matrix.m32, matrix.m33, matrix.m34,
            matrix.m41, matrix.m42, matrix.m43, matrix.m44
        );
    }
}