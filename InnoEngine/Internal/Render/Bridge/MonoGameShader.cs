using InnoEngine.Internal.Render.Impl;

using Microsoft.Xna.Framework.Graphics;

namespace InnoEngine.Internal.Render.Bridge;

internal class MonoGameShader : IShader
{
    private readonly Effect m_effect;

    public MonoGameShader(Effect effect)
    {
        m_effect = effect;
    }

    public void SetTexture(string name, ITexture2D texture)
    {
        if (texture is MonoGameTexture2D mgTex)
            m_effect.Parameters[name]?.SetValue((Texture2D)mgTex.rawTexture);
        else
            throw new ArgumentException("Invalid texture type");
    }

    public void Apply()
    {
        foreach (var pass in m_effect.CurrentTechnique.Passes)
            pass.Apply();
    }
}