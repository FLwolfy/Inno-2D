using Engine.ECS;

namespace Engine.Graphics
{
    /// <summary>
    /// Base class for renderable components supporting Sorting Layer, Order and Z.
    /// </summary>
    public abstract class RenderBehavior : GameBehavior
    {
        public override ComponentTag OrderTag => ComponentTag.Render;
        
        /// <summary>
        /// Sorting layer name. Default "Default".
        /// </summary>
        public string SortingLayer { get; set; } = "Default";

        /// <summary>
        /// Order within sorting layer. Larger means drawn on top.
        /// </summary>
        public int SortingOrder { get; set; } = 0;

        /// <summary>
        /// Z-depth from transform. Used as final tie-breaker for sorting.
        /// </summary>
        public float Z => transform.WorldPosition.Z;

        /// <summary>
        /// Abstract draw method to be implemented by subclasses.
        /// </summary>
        public abstract void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch);
    }
}