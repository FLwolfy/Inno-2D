using Engine.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.ECS.Components
{
    /// <summary>
    /// SpriteRenderer component draws a 2D texture with sorting support.
    /// </summary>
    public class SpriteRenderer : RenderBehavior
    {
        public Texture2D? Sprite { get; set; }
        public Color Color { get; set; } = Color.White;
        public Vector2 Origin { get; set; } = Vector2.Zero;
        public float LayerDepth => 0f; // 可以改用Unity的layerDepth(0~1)映射Z

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Sprite == null || !IsActive)
                return;
            
            Vector3 pos = transform.WorldPosition;
            Vector3 scale = transform.WorldScale;
            
            spriteBatch.Draw(Sprite, new Vector2(pos.X, pos.Y), null, Color, 
                MathHelper.ToRadians(GetZRotationFromQuaternion(transform.WorldRotation)), Origin, new Vector2(scale.X, scale.Y), 
                SpriteEffects.None, LayerDepth);
        }
        
        private float GetZRotationFromQuaternion(Quaternion q)
        {
            // 2D情况下只关心绕Z轴旋转
            // 这里使用公式将四元数转换成欧拉角Z分量（弧度）
            float sinr_cosp = 2 * (q.W * q.Z + q.X * q.Y);
            float cosr_cosp = 1 - 2 * (q.Y * q.Y + q.Z * q.Z);
            return MathF.Atan2(sinr_cosp, cosr_cosp);
        }

    }
}