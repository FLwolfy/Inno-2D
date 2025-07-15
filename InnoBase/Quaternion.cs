using System.Runtime.Serialization;

namespace InnoBase;

[DataContract]
public struct Quaternion : IEquatable<Quaternion>
{
    [DataMember] public float x;
    [DataMember] public float y;
    [DataMember] public float z;
    [DataMember] public float w;

    public Quaternion(float x, float y, float z, float w)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.w = w;
    }

    public static Quaternion identity => new Quaternion(0, 0, 0, 1);

    public float Length() => MathF.Sqrt(x * x + y * y + z * z + w * w);

    public float LengthSquared() => x * x + y * y + z * z + w * w;

    public Quaternion normalized => Normalize(this);

    public static Quaternion Normalize(Quaternion q)
    {
        float len = q.Length();
        if (len < 1e-6f) return identity;
        return new Quaternion(q.x / len, q.y / len, q.z / len, q.w / len);
    }

    public static Quaternion Conjugate(Quaternion q)
        => new Quaternion(-q.x, -q.y, -q.z, q.w);

    public static Quaternion Inverse(Quaternion q)
    {
        float lenSq = q.LengthSquared();
        if (lenSq < 1e-6f) return identity;
        var conj = Conjugate(q);
        return new Quaternion(conj.x / lenSq, conj.y / lenSq, conj.z / lenSq, conj.w / lenSq);
    }

    public static Quaternion Slerp(Quaternion a, Quaternion b, float t)
    {
        float dot = a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;

        if (dot < 0f)
        {
            b = new Quaternion(-b.x, -b.y, -b.z, -b.w);
            dot = -dot;
        }

        if (dot > 0.9995f)
        {
            // Linear interpolation fallback
            Quaternion result = new Quaternion(
                a.x + t * (b.x - a.x),
                a.y + t * (b.y - a.y),
                a.z + t * (b.z - a.z),
                a.w + t * (b.w - a.w)
            );
            return Normalize(result);
        }

        float theta0 = MathF.Acos(dot);
        float sinTheta0 = MathF.Sin(theta0);
        float theta = theta0 * t;
        float sinTheta = MathF.Sin(theta);

        float s0 = MathF.Cos(theta) - dot * sinTheta / sinTheta0;
        float s1 = sinTheta / sinTheta0;

        return new Quaternion(
            a.x * s0 + b.x * s1,
            a.y * s0 + b.y * s1,
            a.z * s0 + b.z * s1,
            a.w * s0 + b.w * s1
        );
    }

    public static Quaternion CreateFromAxisAngle(Vector3 axis, float angle)
    {
        axis = axis.normalized;
        float halfAngle = angle * 0.5f;
        float sin = MathF.Sin(halfAngle);
        return new Quaternion(
            axis.x * sin,
            axis.y * sin,
            axis.z * sin,
            MathF.Cos(halfAngle)
        );
    }

    public static Quaternion CreateFromYawPitchRoll(float yaw, float pitch, float roll)
        => FromEulerAnglesZYX(new Vector3(pitch, yaw, roll));

    public Vector3 ToEulerAnglesXYZ()
    {
        float sinrCosp = 2 * (w * x + y * z);
        float cosrCosp = 1 - 2 * (x * x + y * y);
        float roll = MathF.Atan2(sinrCosp, cosrCosp);

        float sinp = 2 * (w * y - z * x);
        float pitch = MathF.Abs(sinp) >= 1
            ? MathF.CopySign(MathF.PI / 2, sinp)
            : MathF.Asin(sinp);

        float sinyCosp = 2 * (w * z + x * y);
        float cosyCosp = 1 - 2 * (y * y + z * z);
        float yaw = MathF.Atan2(sinyCosp, cosyCosp);

        return new Vector3(roll, pitch, yaw);
    }

    public Vector3 ToEulerAnglesZYX()
    {
        float sinrCosp = 2 * (w * z + x * y);
        float cosrCosp = 1 - 2 * (y * y + z * z);
        float angleZ = MathF.Atan2(sinrCosp, cosrCosp);

        float sinp = 2 * (w * y - z * x);
        float angleY = MathF.Abs(sinp) >= 1
            ? MathF.CopySign(MathF.PI / 2, sinp)
            : MathF.Asin(sinp);

        float sinyCosp = 2 * (w * x + y * z);
        float cosyCosp = 1 - 2 * (x * x + y * y);
        float angleX = MathF.Atan2(sinyCosp, cosyCosp);

        return new Vector3(angleX, angleY, angleZ);
    }

    public static Quaternion FromEulerAnglesXYZ(Vector3 euler)
    {
        float cx = MathF.Cos(euler.x * 0.5f);
        float sx = MathF.Sin(euler.x * 0.5f);
        float cy = MathF.Cos(euler.y * 0.5f);
        float sy = MathF.Sin(euler.y * 0.5f);
        float cz = MathF.Cos(euler.z * 0.5f);
        float sz = MathF.Sin(euler.z * 0.5f);

        return new Quaternion(
            sx * cy * cz + cx * sy * sz,
            cx * sy * cz - sx * cy * sz,
            cx * cy * sz + sx * sy * cz,
            cx * cy * cz - sx * sy * sz
        );
    }

    public static Quaternion FromEulerAnglesZYX(Vector3 euler)
    {
        float cz = MathF.Cos(euler.z * 0.5f);
        float sz = MathF.Sin(euler.z * 0.5f);
        float cy = MathF.Cos(euler.y * 0.5f);
        float sy = MathF.Sin(euler.y * 0.5f);
        float cx = MathF.Cos(euler.x * 0.5f);
        float sx = MathF.Sin(euler.x * 0.5f);

        return new Quaternion(
            sx * cy * cz - cx * sy * sz,
            cx * sy * cz + sx * cy * sz,
            cx * cy * sz - sx * sy * cz,
            cx * cy * cz + sx * sy * sz
        );
    }

    public static Quaternion operator *(Quaternion a, Quaternion b)
    {
        return new Quaternion(
            a.w * b.x + a.x * b.w + a.y * b.z - a.z * b.y,
            a.w * b.y - a.x * b.z + a.y * b.w + a.z * b.x,
            a.w * b.z + a.x * b.y - a.y * b.x + a.z * b.w,
            a.w * b.w - a.x * b.x - a.y * b.y - a.z * b.z
        );
    }

    public static bool operator ==(Quaternion a, Quaternion b)
        => (double) a.x == (double) b.x && (double) a.y == (double) b.y && (double) a.z == (double) b.z && (double) a.w == (double) b.w;

    public static bool operator !=(Quaternion a, Quaternion b)
        => !(a == b);

    public override bool Equals(object? obj) => obj is Quaternion q && this == q;

    public bool Equals(Quaternion other) => this == other;

    public override int GetHashCode()
        => HashCode.Combine(x, y, z, w);

    public override string ToString()
        => $"({x:F3}, {y:F3}, {z:F3}, {w:F3})";
}
