using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Graphics;

/// <summary>
/// Stores texture and sprite-specific render data.
/// </summary>
public class Sprite
{
    public Texture2D texture { get; }
    public Rectangle? sourceRect { get; }
    public Vector2 origin { get; }

    public int width => sourceRect?.Width ?? texture.Width;
    public int height => sourceRect?.Height ?? texture.Height;

    public Sprite(Texture2D texture, Rectangle? sourceRect = null, Vector2? origin = null)
    {
        this.texture = texture;
        this.sourceRect = sourceRect;
        this.origin = origin ?? new Vector2(width / 2f, height / 2f);
    }
}
