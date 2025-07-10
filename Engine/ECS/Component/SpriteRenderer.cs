using Engine.Extension;
using Engine.Graphics.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.ECS.Component;

/// <summary>
/// SpriteRenderer component draws a 2D texture with sorting support.
/// </summary>
public class SpriteRenderer : GameBehavior
{
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
    
    public int layerDepth
    {
        get => m_layerDepth;
        set
        {
            if (m_layerDepth != value)
            {
                m_layerDepth = value;
                
                // TODO: let gameobject has the reference of the scene
                SceneManager.GetActiveScene()?.GetComponentManager().MarkSortDirty(this);
            }
        }
    }
    
    public void Render(SpriteBatch spriteBatch)
    {
        Vector2 pos = new Vector2(transform.worldPosition.X, transform.worldPosition.Y);
        Vector2 scale = new Vector2(transform.worldScale.X, transform.worldScale.Y);
        Color drawColor = color * opacity;
                
        // TODO: make rotation a vector3 not only z
        float rotation = transform.worldRotation.ToEulerAnglesZYX().Z;

        spriteBatch.Draw(
            sprite.texture,
            pos,
            sprite.sourceRect,
            drawColor,
            rotation,
            sprite.origin,
            scale,
            spriteEffects,
            layerDepth);
    }
    
    protected override int Compare(GameComponent other)
    {
        if (other is not SpriteRenderer otherRenderer) {return base.Compare(other);}

        int cmp = layerDepth.CompareTo(otherRenderer.layerDepth);
        if (cmp != 0) return cmp;

        return transform.worldPosition.Z.CompareTo(otherRenderer.transform.worldPosition.Z);
    }
}