using InnoEngine.Internal.Render.Impl;
using Microsoft.Xna.Framework.Graphics;

namespace InnoEngine.Internal.Render.Bridge;

internal class MonoGameTexture2D : ITexture2D
{
    private readonly Texture2D m_texture;

    public MonoGameTexture2D(Texture2D texture)
    {
        m_texture = texture;
    }

    public int width => m_texture.Width;
    public int height => m_texture.Height;

    public Texture2D rawTexture => m_texture;

    public static MonoGameTexture2D LoadFromFile(GraphicsDevice device, string path)
    {
        using var stream = File.OpenRead(path);
        var texture = Texture2D.FromStream(device, stream);
        return new MonoGameTexture2D(texture);
    }
}