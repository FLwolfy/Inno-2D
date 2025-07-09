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

        public Sprite(Texture2D texture, Rectangle? source = null)
        {
            Texture = texture;
            SourceRect = source;
        }
    }
}