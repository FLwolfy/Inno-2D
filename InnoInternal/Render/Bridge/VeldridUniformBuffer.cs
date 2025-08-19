using InnoBase;
using InnoInternal.Render.Impl;

using Veldrid;

namespace InnoInternal.Render.Bridge;

internal class VeldridUniformBuffer : IUniformBuffer
{
    private int m_cursor;
    
    internal byte[] localData { get; private set; }
    internal DeviceBuffer inner { get; }

    public VeldridUniformBuffer(DeviceBuffer buffer)
    {
        inner = buffer;
        localData = new byte[inner.SizeInBytes];
        m_cursor = 0;
    }
    
    public void SetFloat(float value)
    {
        BitConverter.TryWriteBytes(localData.AsSpan(m_cursor), value);
        m_cursor += 4;
    }

    public void SetFloat2(Vector2 value)
    {
        BitConverter.TryWriteBytes(localData.AsSpan(m_cursor), value.x);
        BitConverter.TryWriteBytes(localData.AsSpan(m_cursor + 4), value.y);
        m_cursor += 8;
    }

    public void SetFloat3(Vector3 value)
    {
        BitConverter.TryWriteBytes(localData.AsSpan(m_cursor), value.x);
        BitConverter.TryWriteBytes(localData.AsSpan(m_cursor + 4), value.y);
        BitConverter.TryWriteBytes(localData.AsSpan(m_cursor + 8), value.z);
        m_cursor += 12;
    }

    public void SetFloat4(Vector4 value)
    {
        BitConverter.TryWriteBytes(localData.AsSpan(m_cursor), value.x);
        BitConverter.TryWriteBytes(localData.AsSpan(m_cursor + 4), value.y);
        BitConverter.TryWriteBytes(localData.AsSpan(m_cursor + 8), value.z);
        BitConverter.TryWriteBytes(localData.AsSpan(m_cursor + 12), value.w);
        m_cursor += 16;
    }

    public void SetMatrix(Matrix value)
    {
        var floats = new float[]
        {
            value.m11, value.m12, value.m13, value.m14,
            value.m21, value.m22, value.m23, value.m24,
            value.m31, value.m32, value.m33, value.m34,
            value.m41, value.m42, value.m43, value.m44,
        };
        Buffer.BlockCopy(floats, 0, localData, m_cursor, 64);
        m_cursor += 64;
    }

    public void Dispose()
    {
        inner.Dispose();
    }
}