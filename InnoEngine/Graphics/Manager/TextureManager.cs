using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace InnoEngine.Graphics.Manager;

/// <summary>
/// Manages loading and caching of Texture2D assets.
/// </summary>
public class TextureManager
{
    private readonly GraphicsDevice m_graphicsDevice;
    private readonly ContentManager m_content;
    private readonly Dictionary<string, Texture2D> m_textures = new();

    internal TextureManager(GraphicsDevice graphicsDevice, ContentManager content)
    {
        m_graphicsDevice = graphicsDevice;
        m_content = content;
    }

    /// <summary>
    /// Loads a texture from the content pipeline. Uses cache if already loaded.
    /// </summary>
    public Texture2D Load(string assetName)
    {
        if (m_textures.TryGetValue(assetName, out var cached))
            return cached;

        var texture = m_content.Load<Texture2D>(assetName);
        m_textures[assetName] = texture;
        return texture;
    }

    /// <summary>
    /// Creates a single-color texture.
    /// </summary>
    public Texture2D CreateColorTexture(Color color, int width = 1, int height = 1)
    {
        var texture = new Texture2D(m_graphicsDevice, width, height);
        Color[] data = new Color[width * height];
        for (int i = 0; i < data.Length; i++) data[i] = color;
        texture.SetData(data);
        return texture;
    }

    /// <summary>
    /// Manually register an existing texture with an alias.
    /// </summary>
    public void Register(string key, Texture2D texture)
    {
        m_textures[key] = texture;
    }

    public bool TryGet(string key, out Texture2D texture) => m_textures.TryGetValue(key, out texture);
}
