using System.Runtime.Serialization;

using MGQuaternion = Microsoft.Xna.Framework.Quaternion;

namespace InnoEngine.Base;

[DataContract]
public struct Quaternion : IEquatable<Quaternion>
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

    [DataMember]
    public float w
    {
        get => m_raw.W;
        set => m_raw.W = value;
    }

    private MGQuaternion m_raw;

    public Quaternion(float x, float y, float z, float w)
    {
        m_raw = new MGQuaternion(x, y, z, w);
    }

    private Quaternion(MGQuaternion raw)
    {
        m_raw = raw;
    }

    public static Quaternion identity => new Quaternion(MGQuaternion.Identity);

    public float Length() => m_raw.Length();

    public float LengthSquared() => m_raw.LengthSquared();

    public Quaternion normalized => new Quaternion(MGQuaternion.Normalize(m_raw));

    public static Quaternion CreateFromAxisAngle(Vector3 axis, float angle)
        => new Quaternion(MGQuaternion.CreateFromAxisAngle(axis, angle));

    public static Quaternion CreateFromYawPitchRoll(float yaw, float pitch, float roll)
        => new Quaternion(MGQuaternion.CreateFromYawPitchRoll(yaw, pitch, roll));

    public static Quaternion Conjugate(Quaternion quaternion)
        => new Quaternion(MGQuaternion.Conjugate(quaternion.m_raw));

    public static Quaternion Inverse(Quaternion quaternion)
        => new Quaternion(MGQuaternion.Inverse(quaternion.m_raw));

    public static Quaternion Slerp(Quaternion q1, Quaternion q2, float amount)
        => new Quaternion(MGQuaternion.Slerp(q1.m_raw, q2.m_raw, amount));
    
    /// <summary>
    /// Converts this quaternion to Euler angles in XYZ order (pitch X, yaw Y, roll Z).
    /// The result is in radians.
    /// </summary>
    public Vector3 ToEulerAnglesXYZ()
    {
        double sinrCosp = 2 * (m_raw.W * m_raw.X + m_raw.Y * m_raw.Z);
        double cosrCosp = 1 - 2 * (m_raw.X * m_raw.X + m_raw.Y * m_raw.Y);
        float angleX = (float)Math.Atan2(sinrCosp, cosrCosp);

        double sinp = 2 * (m_raw.W * m_raw.Y - m_raw.Z * m_raw.X);
        float angleY;
        if (Math.Abs(sinp) >= 1)
            angleY = (float)(Math.CopySign(Math.PI / 2, sinp));
        else
            angleY = (float)Math.Asin(sinp);

        double sinyCosp = 2 * (m_raw.W * m_raw.Z + m_raw.X * m_raw.Y);
        double cosyCosp = 1 - 2 * (m_raw.Y * m_raw.Y + m_raw.Z * m_raw.Z);
        float angleZ = (float)Math.Atan2(sinyCosp, cosyCosp);

        return new Vector3(angleX, angleY, angleZ);
    }

    /// <summary>
    /// Converts this quaternion to Euler angles in ZYX order (roll Z, yaw Y, pitch X).
    /// The result is in radians.
    /// </summary>
    public Vector3 ToEulerAnglesZYX()
    {
        double sinrCosp = 2 * (m_raw.W * m_raw.Z + m_raw.X * m_raw.Y);
        double cosrCosp = 1 - 2 * (m_raw.Y * m_raw.Y + m_raw.Z * m_raw.Z);
        float angleZ = (float)Math.Atan2(sinrCosp, cosrCosp);

        double sinp = 2 * (m_raw.W * m_raw.Y - m_raw.Z * m_raw.X);
        float angleY;
        if (Math.Abs(sinp) >= 1)
            angleY = (float)(Math.CopySign(Math.PI / 2, sinp));
        else
            angleY = (float)Math.Asin(sinp);

        double sinyCosp = 2 * (m_raw.W * m_raw.X + m_raw.Y * m_raw.Z);
        double cosyCosp = 1 - 2 * (m_raw.X * m_raw.X + m_raw.Y * m_raw.Y);
        float angleX = (float)Math.Atan2(sinyCosp, cosyCosp);

        return new Vector3(angleX, angleY, angleZ);
    }

    /// <summary>
    /// Creates a quaternion from Euler angles in XYZ order.
    /// </summary>
    public static Quaternion FromEulerAnglesXYZ(Vector3 euler)
    {
        float cx = (float)Math.Cos(euler.x * 0.5);
        float sx = (float)Math.Sin(euler.x * 0.5);
        float cy = (float)Math.Cos(euler.y * 0.5);
        float sy = (float)Math.Sin(euler.y * 0.5);
        float cz = (float)Math.Cos(euler.z * 0.5);
        float sz = (float)Math.Sin(euler.z * 0.5);

        return new Quaternion(
            sx * cy * cz + cx * sy * sz,
            cx * sy * cz - sx * cy * sz,
            cx * cy * sz + sx * sy * cz,
            cx * cy * cz - sx * sy * sz
        );
    }

    /// <summary>
    /// Creates a quaternion from Euler angles in ZYX order.
    /// </summary>
    public static Quaternion FromEulerAnglesZYX(Vector3 euler)
    {
        float cz = (float)Math.Cos(euler.z * 0.5);
        float sz = (float)Math.Sin(euler.z * 0.5);
        float cy = (float)Math.Cos(euler.y * 0.5);
        float sy = (float)Math.Sin(euler.y * 0.5);
        float cx = (float)Math.Cos(euler.x * 0.5);
        float sx = (float)Math.Sin(euler.x * 0.5);

        return new Quaternion(
            sx * cy * cz - cx * sy * sz,
            cx * sy * cz + sx * cy * sz,
            cx * cy * sz - sx * sy * cz,
            cx * cy * cz + sx * sy * sz
        );
    }

    // Operators
    public static Quaternion operator *(Quaternion a, Quaternion b)
        => new Quaternion(a.m_raw * b.m_raw);

    public static bool operator ==(Quaternion a, Quaternion b) => a.m_raw == b.m_raw;
    public static bool operator !=(Quaternion a, Quaternion b) => a.m_raw != b.m_raw;

    public override bool Equals(object? obj)
    {
        if (obj is Quaternion other)
            return m_raw.Equals(other.m_raw);
        return false;
    }

    public bool Equals(Quaternion other) => m_raw.Equals(other.m_raw);

    public override int GetHashCode() => m_raw.GetHashCode();

    public override string ToString() => m_raw.ToString();

    public static implicit operator MGQuaternion(Quaternion q) => q.m_raw;
    public static implicit operator Quaternion(MGQuaternion q) => new Quaternion(q);
}