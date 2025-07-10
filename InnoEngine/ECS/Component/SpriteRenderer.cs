using InnoEngine.Extension;
using InnoEngine.Graphics.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace InnoEngine.ECS.Component;

/// <summary>
/// SpriteRenderer component draws a 2D texture with sorting support.
/// </summary>
public class SpriteRenderer : GameBehavior
{
    private static readonly int MAX_LAYER_DEPTH = 1000;
    
    public override ComponentTag orderTag => ComponentTag.Render;

    public Sprite sprite { get; set; } = Resources.spriteManager.CreateColorSprite(Color.White, 50, 50);
    public Color color { get; set; } = Color.White;
    public SpriteEffects spriteEffects { get; set; } = SpriteEffects.None;
    
    private float m_opacity = 1f;
    private int m_layerDepth = 0;
    
    public float opacity
    {
        get => m_opacity;
        set => m_opacity = MathHelper.Clamp(value, 0f, 1f);
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
            MathHelper.Clamp(value, 0, 1000);
            m_layerDepth = value;
        }
    }
    
    public void Render(SpriteBatch spriteBatch)
    {
        Vector2 pos = new Vector2(transform.worldPosition.X, transform.worldPosition.Y);
        Vector2 scale = new Vector2(transform.worldScale.X, transform.worldScale.Y);
        Color drawColor = color * opacity;
                
        float rotation = transform.worldRotation.ToEulerAnglesZYX().Z;
        float depth = m_layerDepth + (float)((Math.Tanh(transform.worldPosition.Z / MAX_LAYER_DEPTH) + 1.0) / 2.0);

        spriteBatch.Draw(
            sprite.texture,
            pos,
            sprite.sourceRect,
            drawColor,
            rotation,
            sprite.origin,
            scale,
            spriteEffects,
            depth / MAX_LAYER_DEPTH);
    }
}