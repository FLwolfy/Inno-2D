using System.Runtime.Serialization;

using MGVector2 = Microsoft.Xna.Framework.Vector2;

namespace InnoEngine.Base;

[DataContract]
public struct Vector2 : IEquatable<Vector2>
{
    [DataMember]
    public float x
    {
        get => m_raw.X;
        set => m_raw.X = value;
    }

    [DataMember]
    public float y
    {
        get => m_raw.Y;
        set => m_raw.Y = value;
    }

    private MGVector2 m_raw;

    public Vector2(float x, float y)
    {
        m_raw = new MGVector2(x, y);
    }

    private Vector2(MGVector2 raw)
    {
        m_raw = raw;
    }

    public static Vector2 zero => new Vector2(MGVector2.Zero);
    public static Vector2 one => new Vector2(MGVector2.One);
    public static Vector2 unitX => new Vector2(MGVector2.UnitX);
    public static Vector2 unitY => new Vector2(MGVector2.UnitY);

    public float Length() => m_raw.Length();
    public float LengthSquared() => m_raw.LengthSquared();
    
    public Vector2 normalized => new Vector2(MGVector2.Normalize(m_raw));

    public static float Dot(Vector2 a, Vector2 b) => MGVector2.Dot(a.m_raw, b.m_raw);

    public static Vector2 Lerp(Vector2 start, Vector2 end, float amount)
        => new Vector2(MGVector2.Lerp(start.m_raw, end.m_raw, amount));

    public static Vector2 Min(Vector2 a, Vector2 b) => new Vector2(MGVector2.Min(a.m_raw, b.m_raw));

    public static Vector2 Max(Vector2 a, Vector2 b) => new Vector2(MGVector2.Max(a.m_raw, b.m_raw));

    public static Vector2 Reflect(Vector2 vector, Vector2 normal)
        => new Vector2(MGVector2.Reflect(vector.m_raw, normal.m_raw));

    // Operators
    public static Vector2 operator +(Vector2 a, Vector2 b) => new Vector2(a.m_raw + b.m_raw);
    public static Vector2 operator -(Vector2 a, Vector2 b) => new Vector2(a.m_raw - b.m_raw);
    public static Vector2 operator -(Vector2 v) => new Vector2(-v.m_raw);
    public static Vector2 operator *(Vector2 v, float scalar) => new Vector2(v.m_raw * scalar);
    public static Vector2 operator *(float scalar, Vector2 v) => new Vector2(v.m_raw * scalar);
    public static Vector2 operator /(Vector2 v, float scalar) => new Vector2(v.m_raw / scalar);

    public static bool operator ==(Vector2 a, Vector2 b) => a.m_raw == b.m_raw;
    public static bool operator !=(Vector2 a, Vector2 b) => a.m_raw != b.m_raw;

    public override bool Equals(object? obj)
    {
        if (obj is Vector2 other)
            return m_raw.Equals(other.m_raw);
        return false;
    }

    public bool Equals(Vector2 other) => m_raw.Equals(other.m_raw);

    public override int GetHashCode() => m_raw.GetHashCode();

    public override string ToString() => m_raw.ToString();

    public static implicit operator MGVector2(Vector2 v) => v.m_raw;
    public static implicit operator Vector2(MGVector2 v) => new Vector2(v);
}