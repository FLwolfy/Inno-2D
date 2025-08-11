using InnoBase;
using InnoInternal.Render.Impl;

namespace InnoInternal.Render.Bridge;

public class VeldridUniformBuffer : IUniformBuffer
{
    public void Dispose()
    {
        // TODO release managed resources here
    }

    public void SetFloat(string name, float value)
    {
        throw new NotImplementedException();
    }

    public void SetFloat2(string name, Vector2 value)
    {
        throw new NotImplementedException();
    }

    public void SetFloat3(string name, Vector3 value)
    {
        throw new NotImplementedException();
    }

    public void SetFloat4(string name, Vector4 value)
    {
        throw new NotImplementedException();
    }

    public void SetMatrix(string name, Matrix value)
    {
        throw new NotImplementedException();
    }
}