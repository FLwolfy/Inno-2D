using InnoEngine.Base;
using InnoEngine.Resource.Primitive;

namespace InnoEngine.Graphics.RenderCommand;

/// <summary>
/// Represents a batched draw command for rendering a sprite.
/// </summary>
public struct SpriteRenderCommand
{
    public Sprite sprite;
    public Vector2 position;
    public Vector2 scale;
    public Color color;
    public float rotation;
    public float depth;
    public Vector2 origin;
}