using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Graphics
{
    /// <summary>
    /// Stores texture and sprite-specific render data.
    /// </summary>
    public class Sprite
    {
        public Texture2D Texture;
        public Rectangle? SourceRect;
        public Vector2 Pivot = Vector2.Zero;
        public Color Tint = Color.White;

        public Sprite(Texture2D texture)
        {
            Texture = texture;
        }
    }

    /// <summary>
    /// Holds render data extracted from a SpriteRenderer for rendering.
    /// </summary>
    public struct SpriteRenderData
    {
        public Sprite Sprite;
        public Vector2 Position;
        public float Rotation;
        public Vector2 Scale;
        public bool FlipX;
        public bool FlipY;
        public float LayerDepth;
    }
}