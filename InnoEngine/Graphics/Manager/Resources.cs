using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace InnoEngine.Graphics.Manager;

public static class Resources
{
    public static TextureManager textureManager { get; private set; } = null!;
    public static SpriteManager spriteManager { get; private set; } = null!;

    /// <summary>
    /// Initializes the graphics resources manager to instantiate all the resource manager instances.
    /// </summary>
    public static void Initialize(GraphicsDevice device, ContentManager content)
    {
        textureManager = new TextureManager(device, content);
        spriteManager = new SpriteManager(textureManager);
    }
}
