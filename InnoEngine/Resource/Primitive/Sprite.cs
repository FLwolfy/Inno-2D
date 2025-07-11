using InnoEngine.Base;

namespace InnoEngine.Resource.Primitive;

/// <summary>
/// Stores texture and sprite-specific render data.
/// </summary>
public sealed class Sprite
{
    /// <summary>
    /// The underlying 2D texture.
    /// </summary>
    public Texture2D texture { get; }

    /// <summary>
    /// Optional rectangle defining the source region within the texture.
    /// If null, the full texture is used.
    /// </summary>
    public Rect? sourceRect { get; }

    /// <summary>
    /// The pivot/origin point used when rendering this sprite.
    /// </summary>
    public Vector2 origin { get; }

    /// <summary>
    /// The width of the rendered sprite.
    /// </summary>
    public int width => sourceRect?.width ?? texture.width;

    /// <summary>
    /// The height of the rendered sprite.
    /// </summary>
    public int height => sourceRect?.height ?? texture.height;

    /// <summary>
    /// Initializes a new sprite with optional source region and origin.
    /// </summary>
    public Sprite(Texture2D texture, Rect? sourceRect = null, Vector2? origin = null)
    {
        this.texture = texture;
        this.sourceRect = sourceRect;
        this.origin = origin ?? new Vector2(width / 2f, height / 2f);
    }

    public override string ToString() =>
        $"Sprite({width}x{height}, Origin={origin}, SourceRect={(sourceRect?.ToString() ?? "Full")})";
}