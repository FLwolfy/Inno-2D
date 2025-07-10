using System.Collections.Generic;
using System.Linq;
using Engine.ECS;
using Microsoft.Xna.Framework.Graphics;
using Engine.ECS.Components;
using Engine.Graphics.RenderPass;
using Microsoft.Xna.Framework;

namespace Engine.Graphics
{
    /// <summary>
    /// System to update and render all Renderer components in scene.
    /// </summary>
    public class RenderSystem
    {
        private readonly SpriteBatch m_spriteBatch;
        private readonly GraphicsDevice m_graphicsDevice;
        private readonly RenderPipeline m_renderPipeline;
        private readonly RenderContext m_renderContext;

        public RenderSystem(GraphicsDevice device)
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
            m_renderContext.Reset(m_graphicsDevice, m_spriteBatch, gameTime);
            m_renderPipeline.Render(m_renderContext);
        }
    }

}