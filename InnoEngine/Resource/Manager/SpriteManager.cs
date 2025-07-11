using InnoEngine.Resource.Primitive;
using InnoEngine.Base;

namespace InnoEngine.Resource.Manager;

/// <summary>
/// Manages creation and caching of Sprite objects.
/// </summary>
public class SpriteManager
{
    private readonly Texture2DManager m_texture2DManager;
    private readonly Dictionary<string, Sprite> m_sprites = new();

    internal SpriteManager(Texture2DManager texture2DManager)
    {
        m_texture2DManager = texture2DManager;
    }

    /// <summary>
    /// Loads a sprite from a texture asset.
    /// </summary>
    public Sprite Load(string assetName, Rect? sourceRect = null, Vector2? origin = null)
    {
        if (m_sprites.TryGetValue(assetName, out var cached))
            return cached;

        var texture = m_texture2DManager.Load(assetName);
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

        var texture = m_texture2DManager.CreateColorTexture(color, width, height);
        var sprite = new Sprite(texture, null, origin);
        m_sprites[key] = sprite;
        return sprite;
    }

    public bool TryGet(string key, out Sprite? sprite) => m_sprites.TryGetValue(key, out sprite);
}
