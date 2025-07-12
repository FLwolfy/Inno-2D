using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace InnoEngine.Internal.Shell;

public abstract class MonoGameShell : Game
{
    private readonly GraphicsDeviceManager m_graphics;
    private readonly ContentManager m_contents;
        
    protected MonoGameShell()
    {
        m_graphics = new GraphicsDeviceManager(this);
        m_contents = Content;

        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }
        
    protected sealed override void LoadContent()
    {
        Load();
        SetUp();
    }

    protected sealed override void Update(GameTime gameTime)
    {
        Step(gameTime.TotalGameTime.Milliseconds / 1000.0f, gameTime.ElapsedGameTime.Milliseconds / 1000.0f);
        base.Update(gameTime);
    }

    protected sealed override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Gray);
        Draw(gameTime.ElapsedGameTime.Milliseconds / 1000.0f);
        base.Draw(gameTime);
    }

    public abstract void Load();
    public abstract void SetUp();
    public abstract void Step(float totalTime, float deltaTime);
    public abstract void Draw(float deltaTime);
}