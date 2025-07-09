using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Engine.ECS.Components;

namespace Engine.Graphics
{
    /// <summary>
    /// System to update and render all Renderer components in scene.
    /// </summary>
    public class RenderSystem
    {
        private readonly ComponentManager m_componentManager;
        private readonly SpriteBatch m_spriteBatch;

        public RenderSystem(ComponentManager componentManager, SpriteBatch spriteBatch)
        {
            m_componentManager = componentManager;
            m_spriteBatch = spriteBatch;
        }

        public void Render()
        {
            // 获取所有 Renderer 组件
            var renderers = m_componentManager.GetAll<Renderer>().Where(r => r.IsActive).ToList();

            // 排序规则：
            // 1) SortingLayer order ascending（数字小先绘制）
            // 2) SortingOrder ascending（层内顺序，数字小先绘制）
            // 3) Z ascending（物理深度，数字小先绘制）
            renderers.Sort((a, b) =>
            {
                int layerOrderA = SortingLayerManager.GetLayerOrder(a.SortingLayer);
                int layerOrderB = SortingLayerManager.GetLayerOrder(b.SortingLayer);
                int cmp = layerOrderA.CompareTo(layerOrderB);
                if (cmp != 0) return cmp;

                cmp = a.SortingOrder.CompareTo(b.SortingOrder);
                if (cmp != 0) return cmp;

                return a.Z.CompareTo(b.Z);
            });

            // 开启绘制
            m_spriteBatch.Begin();

            foreach (var renderer in renderers)
            {
                renderer.Draw(m_spriteBatch);
            }

            m_spriteBatch.End();
        }
    }
}