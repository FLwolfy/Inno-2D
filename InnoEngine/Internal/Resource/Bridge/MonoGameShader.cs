using InnoEngine.Internal.Resource.Impl;

using Microsoft.Xna.Framework.Graphics;

namespace InnoEngine.Internal.Resource.Bridge;

internal class MonoGameShader : IShader
{
    private readonly Effect m_effect;
    private static Effect? m_defaultEffect;

    public MonoGameShader(Effect effect)
    {
        m_effect = effect;
    }

    public void SetTexture(string name, ITexture2D texture)
    {
        if (texture is MonoGameTexture2D mgTex)
            m_effect.Parameters[name]?.SetValue(mgTex.rawTexture);
        else
            throw new ArgumentException("Invalid texture type");
    }

    public void Apply()
    {
        foreach (var pass in m_effect.CurrentTechnique.Passes)
            pass.Apply();
    }
    
    public static MonoGameShader LoadFromFile(GraphicsDevice device, string? path)
    {
        // Lazy load default effect (e.g., solid color shader)
        if (string.IsNullOrEmpty(path))
        {
            if (m_defaultEffect == null)
            {
                m_defaultEffect = new BasicEffect(device);
            }

            return new MonoGameShader(m_defaultEffect);
        }

        using var stream = File.OpenRead(path);
        var effect = new Effect(device, ReadStreamToByteArray(stream));
        return new MonoGameShader(effect);
    }

    private static byte[] ReadStreamToByteArray(Stream stream)
    {
        using var ms = new MemoryStream();
        stream.CopyTo(ms);
        return ms.ToArray();
    }
}