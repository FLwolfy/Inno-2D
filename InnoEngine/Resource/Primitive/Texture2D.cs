using InnoEngine.Base;

using MGTexture2D = Microsoft.Xna.Framework.Graphics.Texture2D;

namespace InnoEngine.Resource.Primitive;

/// <summary>
/// Represents a 2D texture asset used for rendering.
/// </summary>
public sealed class Texture2D
{
    private readonly MGTexture2D m_native;

    public int width => m_native.Width;
    public int height => m_native.Height;

    public void SetColorData(Color[] colors)
    {
        m_native.SetData(colors.Select(c => c.ToXnaColor()).ToArray());
    }

    public Color[] GetColorData()
    {
        var buffer = new Microsoft.Xna.Framework.Color[width * height];
        m_native.GetData(buffer);
        return buffer.Select(c => new Color(c)).ToArray();
    }

    public override string ToString() => $"Texture2D({width}x{height})";
    
    internal Texture2D(MGTexture2D native)
    {
        m_native = native;
    }

    internal MGTexture2D native => m_native;
}