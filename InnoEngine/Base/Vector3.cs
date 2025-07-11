using System.Runtime.Serialization;

using MGVector3 = Microsoft.Xna.Framework.Vector3;

namespace InnoEngine.Base;

[DataContract]
public struct Vector3 : IEquatable<Vector3>
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

    [DataMember]
    public float z
    {
        get => m_raw.Z;
        set => m_raw.Z = value;
    }

    private MGVector3 m_raw;

    /// <summary>
    /// Constructs a new vector with the given x, y, and z components.
    /// </summary>
    public Vector3(float x, float y, float z)
    {
        m_raw = new MGVector3(x, y, z);
    }

    private Vector3(MGVector3 raw)
    {
        m_raw = raw;
    }
    
    public static Vector3 zero => new Vector3(MGVector3.Zero);
    public static Vector3 one => new Vector3(MGVector3.One);
    
    public static Vector3 unitX => new Vector3(MGVector3.UnitX);
    
    public static Vector3 unitY => new Vector3(MGVector3.UnitY);
    
    public static Vector3 unitZ => new Vector3(MGVector3.UnitZ);
    
    public static Vector3 up => new Vector3(0f, 1f, 0f);
    
    public static Vector3 down => new Vector3(0f, -1f, 0f);
    
    public static Vector3 right => new Vector3(1f, 0f, 0f);
    
    public static Vector3 left => new Vector3(-1f, 0f, 0f);

    /// <summary>
    /// Returns the length of the vector.
    /// </summary>
    public float Length() => m_raw.Length();

    /// <summary>
    /// Returns the squared length of the vector.
    /// </summary>
    public float LengthSquared() => m_raw.LengthSquared();

    /// <summary>
    /// Returns the normalized (unit length) vector.
    /// </summary>
    public Vector3 normalized => new Vector3(MGVector3.Normalize(m_raw));

    /// <summary>
    /// Calculates the Manhattan distance between this vector and another.
    /// </summary>
    public float ManhattanDistance(Vector3 other)
    {
        return MathF.Abs(x - other.x) + MathF.Abs(y - other.y) + MathF.Abs(z - other.z);
    }

    /// <summary>
    /// Returns the dot product of two vectors.
    /// </summary>
    public static float Dot(Vector3 a, Vector3 b) => MGVector3.Dot(a.m_raw, b.m_raw);

    /// <summary>
    /// Returns the cross product of two vectors.
    /// </summary>
    public static Vector3 Cross(Vector3 a, Vector3 b) => new Vector3(MGVector3.Cross(a.m_raw, b.m_raw));

    /// <summary>
    /// Performs a linear interpolation between two vectors.
    /// </summary>
    public static Vector3 Lerp(Vector3 start, Vector3 end, float amount)
        => new Vector3(MGVector3.Lerp(start.m_raw, end.m_raw, amount));

    /// <summary>
    /// Returns the distance between two vectors.
    /// </summary>
    public static float Distance(Vector3 a, Vector3 b) => MGVector3.Distance(a.m_raw, b.m_raw);

    /// <summary>
    /// Returns the squared distance between two vectors.
    /// </summary>
    public static float DistanceSquared(Vector3 a, Vector3 b) => MGVector3.DistanceSquared(a.m_raw, b.m_raw);

    /// <summary>
    /// Returns a vector with all components set to the minimum of each component of the given vectors.
    /// </summary>
    public static Vector3 Min(Vector3 a, Vector3 b) => new Vector3(MGVector3.Min(a.m_raw, b.m_raw));

    /// <summary>
    /// Returns a vector with all components set to the maximum of each component of the given vectors.
    /// </summary>
    public static Vector3 Max(Vector3 a, Vector3 b) => new Vector3(MGVector3.Max(a.m_raw, b.m_raw));

    /// <summary>
    /// Returns a vector with each component rounded to the nearest integer.
    /// </summary>
    public static Vector3 Round(Vector3 v) => new Vector3(MathF.Round(v.x), MathF.Round(v.y), MathF.Round(v.z));

    /// <summary>
    /// Returns the normalized vector of the given vector.
    /// </summary>
    public static Vector3 Normalize(Vector3 v) => new Vector3(MGVector3.Normalize(v.m_raw));

    /// <summary>
    /// Returns the reflection of a vector off a surface with the specified normal.
    /// </summary>
    public static Vector3 Reflect(Vector3 vector, Vector3 normal)
        => new Vector3(MGVector3.Reflect(vector.m_raw, normal.m_raw));
    
    public static Vector3 Transform(Vector3 position, Matrix matrix)
    {
        var result = MGVector3.Transform(position.m_raw, matrix);
        return new Vector3(result);
    }

    public static Vector3 Transform(Vector3 position, Quaternion rotation)
    {
        var result = MGVector3.Transform(position.m_raw, rotation);
        return new Vector3(result);
    }

    // Operator overloads

    public static Vector3 operator +(Vector3 a, Vector3 b) => new Vector3(a.m_raw + b.m_raw);

    public static Vector3 operator -(Vector3 a, Vector3 b) => new Vector3(a.m_raw - b.m_raw);

    public static Vector3 operator -(Vector3 v) => new Vector3(-v.m_raw);

    public static Vector3 operator *(Vector3 v, float scalar) => new Vector3(v.m_raw * scalar);

    public static Vector3 operator *(float scalar, Vector3 v) => new Vector3(v.m_raw * scalar);

    public static Vector3 operator /(Vector3 v, float scalar) => new Vector3(v.m_raw / scalar);

    public static bool operator ==(Vector3 a, Vector3 b) => a.m_raw == b.m_raw;

    public static bool operator !=(Vector3 a, Vector3 b) => a.m_raw != b.m_raw;

    public override bool Equals(object? obj)
    {
        if (obj is Vector3 other)
            return m_raw.Equals(other.m_raw);
        return false;
    }

    public bool Equals(Vector3 other) => m_raw.Equals(other.m_raw);

    public override int GetHashCode() => m_raw.GetHashCode();

    public override string ToString() => m_raw.ToString();

    // Implicit conversions for interop with MonoGame Vector3
    public static implicit operator MGVector3(Vector3 v) => v.m_raw;

    public static implicit operator Vector3(MGVector3 v) => new Vector3(v);
}
