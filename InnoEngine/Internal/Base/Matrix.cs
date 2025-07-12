using System.Runtime.Serialization;

namespace InnoEngine.Internal.Base;

[DataContract]
public struct Matrix : IEquatable<Matrix>
{
    // 4x4 行主序矩阵元素
    [DataMember] public float m11, m12, m13, m14;
    [DataMember] public float m21, m22, m23, m24;
    [DataMember] public float m31, m32, m33, m34;
    [DataMember] public float m41, m42, m43, m44;

    public Matrix(
        float m11, float m12, float m13, float m14,
        float m21, float m22, float m23, float m24,
        float m31, float m32, float m33, float m34,
        float m41, float m42, float m43, float m44)
    {
        this.m11 = m11; this.m12 = m12; this.m13 = m13; this.m14 = m14;
        this.m21 = m21; this.m22 = m22; this.m23 = m23; this.m24 = m24;
        this.m31 = m31; this.m32 = m32; this.m33 = m33; this.m34 = m34;
        this.m41 = m41; this.m42 = m42; this.m43 = m43; this.m44 = m44;
    }

    public static Matrix identity => new Matrix(
        1, 0, 0, 0,
        0, 1, 0, 0,
        0, 0, 1, 0,
        0, 0, 0, 1);

    public static Matrix CreateTranslation(float x, float y, float z)
    {
        return new Matrix(
            1, 0, 0, 0,
            0, 1, 0, 0,
            0, 0, 1, 0,
            x, y, z, 1);
    }

    public static Matrix CreateTranslation(Vector3 v)
        => CreateTranslation(v.x, v.y, v.z);

    public static Matrix CreateScale(float scale)
        => CreateScale(scale, scale, scale);

    public static Matrix CreateScale(float x, float y, float z)
    {
        return new Matrix(
            x, 0, 0, 0,
            0, y, 0, 0,
            0, 0, z, 0,
            0, 0, 0, 1);
    }

    public static Matrix CreateScale(Vector3 v)
        => CreateScale(v.x, v.y, v.z);

    public static Matrix CreateRotationX(float radians)
    {
        float c = MathF.Cos(radians);
        float s = MathF.Sin(radians);
        return new Matrix(
            1, 0, 0, 0,
            0, c, s, 0,
            0, -s, c, 0,
            0, 0, 0, 1);
    }

    public static Matrix CreateRotationY(float radians)
    {
        float c = MathF.Cos(radians);
        float s = MathF.Sin(radians);
        return new Matrix(
            c, 0, -s, 0,
            0, 1, 0, 0,
            s, 0, c, 0,
            0, 0, 0, 1);
    }

    public static Matrix CreateRotationZ(float radians)
    {
        float c = MathF.Cos(radians);
        float s = MathF.Sin(radians);
        return new Matrix(
            c, s, 0, 0,
            -s, c, 0, 0,
            0, 0, 1, 0,
            0, 0, 0, 1);
    }

    public static Matrix CreateFromQuaternion(Quaternion q)
    {
        float xx = q.x * q.x, yy = q.y * q.y, zz = q.z * q.z;
        float xy = q.x * q.y, xz = q.x * q.z, yz = q.y * q.z;
        float wx = q.w * q.x, wy = q.w * q.y, wz = q.w * q.z;

        return new Matrix(
            1 - 2 * (yy + zz), 2 * (xy + wz),     2 * (xz - wy),     0,
            2 * (xy - wz),     1 - 2 * (xx + zz), 2 * (yz + wx),     0,
            2 * (xz + wy),     2 * (yz - wx),     1 - 2 * (xx + yy), 0,
            0,                 0,                 0,                 1);
    }

    public static Matrix CreatePerspectiveFieldOfView(float fov, float aspect, float near, float far)
    {
        float f = 1f / MathF.Tan(fov / 2);
        float nf = 1f / (near - far);

        return new Matrix(
            f / aspect, 0, 0,                         0,
            0,          f, 0,                         0,
            0,          0, (far + near) * nf,        -1,
            0,          0, (2 * far * near) * nf,     0);
    }

    public static Matrix CreateOrthographic(float width, float height, float near, float far)
    {
        float rl = 1f / width;
        float tb = 1f / height;
        float fn = 1f / (far - near);

        return new Matrix(
            2 * rl, 0,      0,              0,
            0,      2 * tb, 0,              0,
            0,      0,     -2 * fn,         0,
           -0,     -0,     -(far + near) * fn, 1);
    }

    public static Matrix CreateLookAt(Vector3 eye, Vector3 target, Vector3 up)
    {
        Vector3 z = (eye - target).normalized;
        Vector3 x = Vector3.Cross(up, z).normalized;
        Vector3 y = Vector3.Cross(z, x);

        return new Matrix(
            x.x, y.x, z.x, 0,
            x.y, y.y, z.y, 0,
            x.z, y.z, z.z, 0,
            -Vector3.Dot(x, eye), -Vector3.Dot(y, eye), -Vector3.Dot(z, eye), 1);
    }

    public static Matrix Multiply(Matrix a, Matrix b)
    {
        return new Matrix(
            a.m11 * b.m11 + a.m12 * b.m21 + a.m13 * b.m31 + a.m14 * b.m41,
            a.m11 * b.m12 + a.m12 * b.m22 + a.m13 * b.m32 + a.m14 * b.m42,
            a.m11 * b.m13 + a.m12 * b.m23 + a.m13 * b.m33 + a.m14 * b.m43,
            a.m11 * b.m14 + a.m12 * b.m24 + a.m13 * b.m34 + a.m14 * b.m44,

            a.m21 * b.m11 + a.m22 * b.m21 + a.m23 * b.m31 + a.m24 * b.m41,
            a.m21 * b.m12 + a.m22 * b.m22 + a.m23 * b.m32 + a.m24 * b.m42,
            a.m21 * b.m13 + a.m22 * b.m23 + a.m23 * b.m33 + a.m24 * b.m43,
            a.m21 * b.m14 + a.m22 * b.m24 + a.m23 * b.m34 + a.m24 * b.m44,

            a.m31 * b.m11 + a.m32 * b.m21 + a.m33 * b.m31 + a.m34 * b.m41,
            a.m31 * b.m12 + a.m32 * b.m22 + a.m33 * b.m32 + a.m34 * b.m42,
            a.m31 * b.m13 + a.m32 * b.m23 + a.m33 * b.m33 + a.m34 * b.m43,
            a.m31 * b.m14 + a.m32 * b.m24 + a.m33 * b.m34 + a.m34 * b.m44,

            a.m41 * b.m11 + a.m42 * b.m21 + a.m43 * b.m31 + a.m44 * b.m41,
            a.m41 * b.m12 + a.m42 * b.m22 + a.m43 * b.m32 + a.m44 * b.m42,
            a.m41 * b.m13 + a.m42 * b.m23 + a.m43 * b.m33 + a.m44 * b.m43,
            a.m41 * b.m14 + a.m42 * b.m24 + a.m43 * b.m34 + a.m44 * b.m44
        );
    }

    public static Matrix operator *(Matrix a, Matrix b) => Multiply(a, b);

    public static bool operator ==(Matrix matrix1, Matrix matrix2)
    {
        return (double) matrix1.m11 == (double) matrix2.m11 && (double) matrix1.m12 == (double) matrix2.m12 && (double) matrix1.m13 == (double) matrix2.m13 && (double) matrix1.m14 == (double) matrix2.m14 && (double) matrix1.m21 == (double) matrix2.m21 && (double) matrix1.m22 == (double) matrix2.m22 && (double) matrix1.m23 == (double) matrix2.m23 && (double) matrix1.m24 == (double) matrix2.m24 && (double) matrix1.m31 == (double) matrix2.m31 && (double) matrix1.m32 == (double) matrix2.m32 && (double) matrix1.m33 == (double) matrix2.m33 && (double) matrix1.m34 == (double) matrix2.m34 && (double) matrix1.m41 == (double) matrix2.m41 && (double) matrix1.m42 == (double) matrix2.m42 && (double) matrix1.m43 == (double) matrix2.m43 && (double) matrix1.m44 == (double) matrix2.m44;
    }

    public static bool operator !=(Matrix a, Matrix b) => !(a == b);

    public bool Equals(Matrix other) => this == other;

    public override bool Equals(object? obj) => obj is Matrix other && Equals(other);

    public override int GetHashCode()
    {
        return this.m11.GetHashCode() + this.m12.GetHashCode() + this.m13.GetHashCode() + this.m14.GetHashCode() + this.m21.GetHashCode() + this.m22.GetHashCode() + this.m23.GetHashCode() + this.m24.GetHashCode() + this.m31.GetHashCode() + this.m32.GetHashCode() + this.m33.GetHashCode() + this.m34.GetHashCode() + this.m41.GetHashCode() + this.m42.GetHashCode() + this.m43.GetHashCode() + this.m44.GetHashCode();
    }

    public override string ToString()
    {
        return $"[{m11}, {m12}, {m13}, {m14}]\n" +
               $"[{m21}, {m22}, {m23}, {m24}]\n" +
               $"[{m31}, {m32}, {m33}, {m34}]\n" +
               $"[{m41}, {m42}, {m43}, {m44}]";
    }
}
