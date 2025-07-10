using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Graphics.Manager;

/// <summary>
/// Manages creation and caching of Sprite objects.
/// </summary>
public class SpriteManager
{
    private readonly TextureManager m_textureManager;
    private readonly Dictionary<string, Sprite> m_sprites = new();

    internal SpriteManager(TextureManager textureManager)
    {
        m_textureManager = textureManager;
    }

    /// <summary>
    /// Loads a sprite from a texture asset.
    /// </summary>
    public Sprite Load(string assetName, Rectangle? sourceRect = null, Vector2? origin = null)
    {
        if (m_sprites.TryGetValue(assetName, out var cached))
            return cached;

        var texture = m_textureManager.Load(assetName);
        var sprite = new Sprite(texture, sourceRect, origin);
        m_sprites[assetName] = sprite;
        return sprite;
    }

    /// <summary>
    /// Creates a sprite from a solid color texture.
    /// </summary>
    public Sprite CreateColorSprite(Color color, int width = 1, int height = 1, Vector2? origin = null)
    {
        string key = $"__color_{color}_{width}x{height}";
        if (m_sprites.TryGetValue(key, out var cached)) return cached;

        var texture = m_textureManager.CreateColorTexture(color, width, height);
        var sprite = new Sprite(texture, null, origin);
        m_sprites[key] = sprite;
        return sprite;
    }

    public bool TryGet(string key, out Sprite sprite) => m_sprites.TryGetValue(key, out sprite);
}

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
