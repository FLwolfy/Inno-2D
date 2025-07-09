using System.Collections.Generic;
using System.Linq;
using Engine.ECS;
using Microsoft.Xna.Framework.Graphics;
using Engine.ECS.Components;
using Microsoft.Xna.Framework;

namespace Engine.Graphics
{
    /// <summary>
    /// System to update and render all Renderer components in scene.
    /// </summary>
    public class RenderSystem
    {
        private SpriteBatch _spriteBatch;
        private GraphicsDevice _graphicsDevice;

        public RenderSystem(GraphicsDevice device)
        {
            _graphicsDevice = device;
            _spriteBatch = new SpriteBatch(device);
        }

        public void RenderScene(GameScene scene)
        {
            _graphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);

            // TODO: rendering order by sorting layer and orderZ

            _spriteBatch.End();
        }
    }

}