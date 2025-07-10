using Microsoft.Xna.Framework.Graphics;
using InnoEngine.ECS;
using InnoEngine.Graphics.RenderPass;
using Microsoft.Xna.Framework;

namespace InnoEngine.Graphics
{
    /// <summary>
    /// System to update and render all Renderer components in scene.
    /// </summary>
    public class RenderSystem
    {
        private SpriteBatch m_spriteBatch = null!;
        private GraphicsDevice m_graphicsDevice = null!;
        private RenderPipeline m_renderPipeline = null!;
        private RenderContext m_renderContext = null!;

        public void Initialize(GraphicsDevice device)
        {
            m_graphicsDevice = device;
            m_spriteBatch = new SpriteBatch(device);
            m_renderPipeline = new RenderPipeline();
            m_renderContext = new RenderContext();
        }

        public void LoadRenderPasses()
        {
            // TODO: Add more RenderPasses here
            m_renderPipeline.Register(new ClearScreenPass());
            m_renderPipeline.Register(new SpriteRenderPass());
        }

        public void RenderScene(GameScene scene, GameTime gameTime)
        {
            m_renderContext.Reset(m_graphicsDevice, m_spriteBatch, gameTime, scene);
            m_renderPipeline.Render(m_renderContext);
        }
    }

}