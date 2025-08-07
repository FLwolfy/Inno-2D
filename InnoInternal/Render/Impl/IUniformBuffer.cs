using InnoBase;

namespace InnoInternal.Render.Impl;

public interface IUniformBuffer : IDisposable
{
    void SetFloat(string name, float value);
    void SetFloat2(string name, Vector2 value);
    void SetFloat3(string name, Vector3 value);
    void SetFloat4(string name, Vector4 value);
    void SetMatrix(string name, Matrix value);
    
    // TODO: Add more type supports
}