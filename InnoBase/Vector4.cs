using System.Runtime.Serialization;

namespace InnoBase;

[DataContract]
public struct Vector4 : IEquatable<Vector4>
{
    [DataMember] public float x;
    [DataMember] public float y;
    [DataMember] public float z;
    [DataMember] public float w;

    public Vector4(float x, float y, float z, float w)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.w = w;
    }

    // Common vectors
    public static readonly Vector4 ZERO = new(0, 0, 0, 0);
    public static readonly Vector4 ONE = new(1, 1, 1, 1);
    public static readonly Vector4 UNIT_X = new(1, 0, 0, 0);
    public static readonly Vector4 UNIT_Y = new(0, 1, 0, 0);
    public static readonly Vector4 UNIT_Z = new(0, 0, 1, 0);
    public static readonly Vector4 UNIT_W = new(0, 0, 0, 1);

    // Length
    public float Length() => MathF.Sqrt(x * x + y * y + z * z + w * w);
    public float LengthSquared() => x * x + y * y + z * z + w * w;

    public Vector4 normalized
    {
        get
        {
            float len = Length();
            return len > 0 ? this / len : ZERO;
        }
    }

    // Dot product
    public static float Dot(Vector4 a, Vector4 b) =>
        a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;

    // Lerp
    public static Vector4 Lerp(Vector4 a, Vector4 b, float t) =>
        a + (b - a) * Mathematics.Clamp(t, 0f, 1f);

    // Reflect
    public static Vector4 Reflect(Vector4 vec, Vector4 normal) =>
        vec - 2f * Dot(vec, normal) * normal;

    // Transform by Matrix (assumes Vector4 is row vector)
    public static Vector4 Transform(Vector4 v, Matrix m)
    {
        float tx = v.x * m.m11 + v.y * m.m21 + v.z * m.m31 + v.w * m.m41;
        float ty = v.x * m.m12 + v.y * m.m22 + v.z * m.m32 + v.w * m.m42;
        float tz = v.x * m.m13 + v.y * m.m23 + v.z * m.m33 + v.w * m.m43;
        float tw = v.x * m.m14 + v.y * m.m24 + v.z * m.m34 + v.w * m.m44;
        return new Vector4(tx, ty, tz, tw);
    }

    // Project (useful in homogeneous coordinate systems)
    public Vector3 ProjectToVector3()
    {
        if (w == 0f) return new Vector3(x, y, z); // Avoid division by zero
        return new Vector3(x / w, y / w, z / w);
    }

    // Operators
    public static Vector4 operator +(Vector4 a, Vector4 b) =>
        new(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);

    public static Vector4 operator -(Vector4 a, Vector4 b) =>
        new(a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w);

    public static Vector4 operator -(Vector4 v) =>
        new(-v.x, -v.y, -v.z, -v.w);

    public static Vector4 operator *(Vector4 v, float s) =>
        new(v.x * s, v.y * s, v.z * s, v.w * s);

    public static Vector4 operator *(float s, Vector4 v) => v * s;

    public static Vector4 operator /(Vector4 v, float s) =>
        new(v.x / s, v.y / s, v.z / s, v.w / s);

    public static bool operator ==(Vector4 a, Vector4 b) =>
        MathF.Abs(a.x - b.x) < 1e-6f &&
        MathF.Abs(a.y - b.y) < 1e-6f &&
        MathF.Abs(a.z - b.z) < 1e-6f &&
        MathF.Abs(a.w - b.w) < 1e-6f;

    public static bool operator !=(Vector4 a, Vector4 b) => !(a == b);

    public override bool Equals(object? obj) => obj is Vector4 other && this == other;
    public bool Equals(Vector4 other) => this == other;
    public override int GetHashCode() => HashCode.Combine(x, y, z, w);
    public override string ToString() => $"({x}, {y}, {z}, {w})";
}
