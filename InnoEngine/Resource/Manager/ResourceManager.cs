using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace InnoEngine.Resource.Manager;

public static class ResourceManager
{
    public static Texture2DManager texture2DManager { get; private set; } = null!;
    public static SpriteManager spriteManager { get; private set; } = null!;

    /// <summary>
    /// Initializes the graphics resources manager to instantiate all the resource manager instances.
    /// </summary>
    internal static void Initialize(GraphicsDevice device, ContentManager content)
    {
        texture2DManager = new Texture2DManager(device, content);
        spriteManager = new SpriteManager(texture2DManager);
    }
}
