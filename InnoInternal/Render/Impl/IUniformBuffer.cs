using InnoBase;

namespace InnoInternal.Render.Impl;

public interface IUniformBuffer : IDisposable
{
    void SetFloat(float value);
    void SetFloat2(Vector2 value);
    void SetFloat3(Vector3 value);
    void SetFloat4(Vector4 value);
    void SetMatrix(Matrix value);
    
    // TODO: Add more type supports
}