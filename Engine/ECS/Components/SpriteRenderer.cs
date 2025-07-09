using Engine.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.ECS.Components
{
    /// <summary>
    /// SpriteRenderer component draws a 2D texture with sorting support.
    /// </summary>
    public class SpriteRenderer : GameBehavior, IRenderable
    {
        public Sprite Sprite;
        public Vector2 Origin;
        public Color Color = Color.White;
        public float Layer = 0f; // Z-Sorting

        public override ComponentTag OrderTag => ComponentTag.Render;

        public void Render(SpriteBatch spriteBatch)
        {
            var transform = this.transform;

            spriteBatch.Draw(
                Sprite.Texture,
                new Vector2(transform.WorldPosition.X, transform.WorldPosition.Y),
                Sprite.SourceRect,
                Color,
                Sprite.RotationZ,
                Origin,
                new Vector2(transform.WorldScale.X, transform.WorldScale.Y),
                SpriteEffects.None,
                Layer
            );
        }

    }

}