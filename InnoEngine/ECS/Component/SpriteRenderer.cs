using InnoEngine.Base;
using InnoEngine.Graphics.RenderCommand;
using InnoEngine.Resource.Manager;
using InnoEngine.Resource.Primitive;

namespace InnoEngine.ECS.Component;

/// <summary>
/// SpriteRenderer component draws a 2D texture with sorting support.
/// </summary>
public class SpriteRenderer : GameBehavior
{
    private static readonly int MAX_LAYER_DEPTH = 1000;
    
    public override ComponentTag orderTag => ComponentTag.Render;

    public Sprite sprite { get; set; } = ResourceManager.spriteManager.CreateColorSprite(Color.WHITE, 50, 50);
    public Color color { get; set; } = Color.WHITE;
    
    private float m_opacity = 1f;
    private int m_layerDepth = 0;
    
    public float opacity
    {
        get => m_opacity;
        set => m_opacity = Mathematics.Clamp(value, 0f, 1f);
    }
    
    /// <summary>
    /// Layer depth for sorting sprites. It is ranged from 0 to 1000, where 0 is the lowest layer and 1000 is the highest layer.
    /// </summary>
    public int layerDepth
    {
        get => m_layerDepth;
        set
        {
            if (m_layerDepth == value) return;
            Mathematics.Clamp(value, 0, 1000);
            m_layerDepth = value;
        }
    }
    
    internal SpriteRenderCommand GenerateRenderCommand()
    {
        return new SpriteRenderCommand
        {
            sprite = sprite,
            position = new Vector2(transform.worldPosition.x, transform.worldPosition.y),
            scale = new Vector2(transform.worldScale.x, transform.worldScale.y),
            rotation = transform.worldRotation.ToEulerAnglesZYX().z,
            depth = m_layerDepth + (float)((Math.Tanh(transform.worldPosition.z / MAX_LAYER_DEPTH) + 1.0) / 2.0) / 1000.0f,
            color = color * opacity,
            origin = sprite.origin
        };
    }

}