using Engine.ECS;
using Engine.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Engine.Core
{
    public class GameShell : Game
    {
        private GraphicsDeviceManager m_graphics;
        private RenderSystem m_renderSystem;

        public GameShell()
        {
            m_graphics = new GraphicsDeviceManager(this);
            m_renderSystem = new RenderSystem(GraphicsDevice);

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void LoadContent()
        {
            // TODO: Load game assets and scenes here
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            
            SceneManager.GetActiveScene().Update();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            var scene = SceneManager.GetActiveScene();
            m_renderSystem.RenderScene(scene);

            base.Draw(gameTime);
        }
    }
}