using System.Runtime.Serialization;

namespace InnoBase;

[DataContract]
public struct Vector2 : IEquatable<Vector2>
{
    [DataMember] public float x;
    [DataMember] public float y;

    public Vector2(float x, float y)
    {
        this.x = x;
        this.y = y;
    }

    public static readonly Vector2 ZERO = new(0f, 0f);
    public static readonly Vector2 ONE = new(1f, 1f);
    public static readonly Vector2 UNIT_X = new(1f, 0f);
    public static readonly Vector2 UNIT_Y = new(0f, 1f);

    public float Length() => MathF.Sqrt(x * x + y * y);
    public float LengthSquared() => x * x + y * y;

    public Vector2 normalized
    {
        get
        {
            float len = Length();
            return len > 0f ? this / len : ZERO;
        }
    }

    public static float Dot(Vector2 a, Vector2 b) => a.x * b.x + a.y * b.y;

    public static Vector2 Lerp(Vector2 a, Vector2 b, float t)
        => new Vector2(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t);

    public static Vector2 Min(Vector2 a, Vector2 b)
        => new Vector2(MathF.Min(a.x, b.x), MathF.Min(a.y, b.y));

    public static Vector2 Max(Vector2 a, Vector2 b)
        => new Vector2(MathF.Max(a.x, b.x), MathF.Max(a.y, b.y));

    public static Vector2 Reflect(Vector2 v, Vector2 n)
        => v - 2f * Dot(v, n) * n;

    // Operators
    public static Vector2 operator +(Vector2 a, Vector2 b) => new Vector2(a.x + b.x, a.y + b.y);
    public static Vector2 operator -(Vector2 a, Vector2 b) => new Vector2(a.x - b.x, a.y - b.y);
    public static Vector2 operator -(Vector2 v) => new Vector2(-v.x, -v.y);
    public static Vector2 operator *(Vector2 v, float scalar) => new Vector2(v.x * scalar, v.y * scalar);
    public static Vector2 operator *(float scalar, Vector2 v) => v * scalar;
    public static Vector2 operator /(Vector2 v, float scalar) => new Vector2(v.x / scalar, v.y / scalar);

    public static bool operator ==(Vector2 a, Vector2 b) => a.x == b.x && a.y == b.y;
    public static bool operator !=(Vector2 a, Vector2 b) => !(a == b);

    public override bool Equals(object? obj) => obj is Vector2 other && Equals(other);
    public bool Equals(Vector2 other) => x == other.x && y == other.y;
    public override int GetHashCode() => HashCode.Combine(x, y);
    public override string ToString() => $"({x:F2}, {y:F2})";
}
