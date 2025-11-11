using InnoBase;
using InnoBase.Math;

namespace InnoEngine.Graphics.Shader;

public static class ShaderVertexLayout
{
    public struct VertexPosition(Vector3 position)
    {
        public Vector3 position = position;
    }
    
    public struct VertexPositionTexture(Vector3 position, Vector2 texCoord)
    {
        public Vector3 position = position;
        public Vector2 texCoord = texCoord;
    }
}