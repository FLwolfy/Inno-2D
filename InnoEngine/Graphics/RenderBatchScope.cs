using Microsoft.Xna.Framework.Graphics;

namespace InnoEngine.Graphics;

/// <summary>
/// Helper to simplify SpriteBatch begin/end lifecycle.
/// </summary>
public class RenderBatchScope : IDisposable
{
    private readonly SpriteBatch m_spriteBatch;

    /// <summary>
    /// Using this scope will automatically call Begin() on the SpriteBatch, and call End() when the scope ends.
    /// </summary>
    public RenderBatchScope(SpriteBatch spriteBatch, SpriteSortMode sortMode = SpriteSortMode.Deferred)
    {
        m_spriteBatch = spriteBatch;
        m_spriteBatch.Begin(sortMode, BlendState.AlphaBlend);
    }

    public void Dispose() => m_spriteBatch.End();
}