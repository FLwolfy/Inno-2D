using Microsoft.Xna.Framework.Graphics;

namespace Engine.Graphics;

/// <summary>
/// Helper to simplify SpriteBatch begin/end lifecycle.
/// </summary>
public class RenderBatchScope : IDisposable
{
    private readonly SpriteBatch m_spriteBatch;

    public RenderBatchScope(SpriteBatch spriteBatch, SpriteSortMode sortMode = SpriteSortMode.Deferred)
    {
        m_spriteBatch = spriteBatch;
        m_spriteBatch.Begin(sortMode, BlendState.AlphaBlend);
    }

    public void Dispose() => m_spriteBatch.End();
}