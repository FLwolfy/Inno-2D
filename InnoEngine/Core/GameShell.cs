using InnoEngine.ECS;
using InnoEngine.Graphics;
using InnoEngine.Graphics.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace InnoEngine.Core;

public abstract class GameShell : Game
{
    private readonly GraphicsDeviceManager m_graphics;
    private readonly ContentManager m_contents;
    private readonly RenderSystem m_renderSystem;

    protected GameShell()
    {
        m_graphics = new GraphicsDeviceManager(this);
        m_contents = Content;
        m_renderSystem = new RenderSystem();
        
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected sealed override void LoadContent()
    {
        // Render Initialization
        Resources.Initialize(GraphicsDevice, m_contents);
        m_renderSystem.Initialize(GraphicsDevice);
        m_renderSystem.LoadRenderPasses();
        
        // Load Contents
        SetUp();
        
        // Start all the loaded scenes
        foreach (var scene in SceneManager.GetAllScenes()) { scene.Start(); }
    }

    protected sealed override void Update(GameTime gameTime)
    {
        if (Keyboard.GetState().IsKeyDown(Keys.Escape)) { Exit();}
        SceneManager.GetActiveScene()?.Update();
        base.Update(gameTime);
    }

    protected sealed override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.LightGray);

        var scene = SceneManager.GetActiveScene();
        if (scene == null) { return; }
        m_renderSystem.RenderScene(scene, gameTime);

        base.Draw(gameTime);
    }

    public abstract void SetUp();
    
}
