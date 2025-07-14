using InnoEngine.Internal.Render.Impl;

using Microsoft.Xna.Framework.Graphics;

namespace InnoEngine.Internal.Render.Bridge;

internal class MonoGameRenderAPI : IRenderAPI
{
    public IRenderContext context { get; private set; }
    public IRenderCommand command { get; private set; }
    public ISpriteBatch spriteBatch { get; private set; }

    public void Initialize(object data)
    {
        if (data is not GraphicsDevice device)
        {
            throw new ArgumentException("Invalid data type. Expected GraphicsDevice.", nameof(data));
        }
        
        context = new MonoGameRenderContext(device);
        command = new MonoGameRenderCommand(device);
        spriteBatch = new MonoGameSpriteBatch(device);
    }
}