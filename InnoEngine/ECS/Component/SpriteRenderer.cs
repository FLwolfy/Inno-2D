using InnoBase;
using InnoEngine.Graphics.RenderObject;

namespace InnoEngine.ECS.Component;

/// <summary>
/// SpriteRenderer component draws a 2D texture with sorting support.
/// </summary>
public class SpriteRenderer : GameBehavior
{
    private static readonly int MAX_LAYER_DEPTH = 1000;
    
    public override ComponentTag orderTag => ComponentTag.Render;

    public Sprite sprite { get; set; } = new Sprite();
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
            Mathematics.Clamp(value, 0, MAX_LAYER_DEPTH);
            m_layerDepth = value;
        }
    }
    
    internal struct SpriteRenderCommand
    {
        public Sprite sprite;
        public Vector2 position;
        public Vector2 scale;
        public float rotation;
        public float depth;
        public Color color;
        public Vector2 origin;
    }
    
    internal SpriteRenderCommand GenerateRenderCommand()
    {
        return new SpriteRenderCommand
        {
            sprite = sprite,
            position = new Vector2(transform.worldPosition.x, transform.worldPosition.y),
            scale = new Vector2(transform.worldScale.x, transform.worldScale.y),
            rotation = transform.worldRotation.ToEulerAnglesZYX().z,
            depth = m_layerDepth + (float)((Math.Tanh(transform.worldPosition.z / MAX_LAYER_DEPTH) + 1.0) / 2.0) / MAX_LAYER_DEPTH, // TODO: This should be clamped within [0, 1]
            color = color * opacity,
            origin = sprite.origin
        };
    }

}