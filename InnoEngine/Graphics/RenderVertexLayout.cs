using InnoBase;

namespace InnoEngine.Graphics;

public static class RenderVertexLayout
{
    public struct VertexPosition(Vector3 position)
    {
        public Vector3 position = position;
    }
    
    public struct VertexPositionColor(Vector3 position, Color color)
    {
        public Vector3 position = position;
        public Color color = color;
    }
}