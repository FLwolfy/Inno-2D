using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using MGColor = Microsoft.Xna.Framework.Color;
using MGTexture2D = Microsoft.Xna.Framework.Graphics.Texture2D;
using Color = InnoEngine.Internal.Base.Color;
using Texture2D = InnoEngine.Resource.Primitive.Texture2D;

namespace InnoEngine.Resource.Manager;

/// <summary>
/// Manages loading and caching of Texture2D assets.
/// </summary>
public class Texture2DManager
{
    private readonly GraphicsDevice m_graphicsDevice;
    private readonly ContentManager m_content;
    private readonly Dictionary<string, Texture2D> m_textures = new();

    internal Texture2DManager(GraphicsDevice graphicsDevice, ContentManager content)
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

        var mgTexture2D = m_content.Load<MGTexture2D>(assetName);
        var texture2D = new Texture2D(mgTexture2D);
        
        m_textures[assetName] = texture2D;
        return texture2D;
    }

    /// <summary>
    /// Creates a single-color texture.
    /// </summary>
    public Texture2D CreateColorTexture(Color color, int width = 1, int height = 1)
    {
        var texture = new MGTexture2D(m_graphicsDevice, width, height);
        MGColor[] data = new MGColor[width * height];
        for (int i = 0; i < data.Length; i++) data[i] = color.ToXnaColor();
        texture.SetData(data);
        return new Texture2D(texture);
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
