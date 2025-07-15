using InnoInternal.Render.Impl;
using Microsoft.Xna.Framework.Graphics;

namespace InnoInternal.Render.Bridge;

internal class MonoGameRenderAPI : IRenderAPI
{
    internal static GraphicsDevice graphicsDevice { get; private set; } = null!;
    
    public IRenderContext context { get; private set; } = null!;
    public IRenderCommand command { get; private set; } = null!;
    public ISpriteBatch spriteBatch { get; private set; } = null!;

    public void Initialize(object device)
    {
        if (device is not GraphicsDevice)
        {
            throw new ArgumentException("Invalid data type. Expected GraphicsDevice.", nameof(device));
        }
        
        graphicsDevice = (GraphicsDevice)device;
        
        context = new MonoGameRenderContext();
        command = new MonoGameRenderCommand();
        spriteBatch = new MonoGameSpriteBatch();
    }
}