using System.Reflection;
using InnoInternal.Render.Bridge;
using InnoInternal.Resource.Impl;
using Microsoft.Xna.Framework.Graphics;

namespace InnoInternal.Resource.Bridge;

internal class MonoGameTexture2D : ITexture2D
{
    private static Texture2D? m_defaultWhiteTex;
    private readonly Texture2D m_texture;
    
    private static GraphicsDevice device => MonoGameRenderAPI.graphicsDevice;

    public MonoGameTexture2D(Texture2D texture)
    {
        m_texture = texture;
    }

    public int width => m_texture.Width;
    public int height => m_texture.Height;

    public Texture2D rawTexture => m_texture;

    public static MonoGameTexture2D LoadFromFile(string? path)
    {
        // Lazy load default texture (e.g., white pixel texture)
        if (string.IsNullOrEmpty(path))
        {
            if (m_defaultWhiteTex == null)
            {
                // TODO: Use global PixelPerUnit
                m_defaultWhiteTex = new Texture2D(device, 1, 1);
                m_defaultWhiteTex.SetData([Microsoft.Xna.Framework.Color.White]);
            }

            return new MonoGameTexture2D(m_defaultWhiteTex);
        }

        using var stream = File.OpenRead(path);
        var texture = Texture2D.FromStream(device, stream);
        return new MonoGameTexture2D(texture);
    }

}