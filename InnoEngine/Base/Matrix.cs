using System.Runtime.Serialization;

using MGMatrix = Microsoft.Xna.Framework.Matrix;

namespace InnoEngine.Base;

[DataContract]
public struct Matrix : IEquatable<Matrix>
{
    // Backing MonoGame Matrix
    [DataMember]
    private MGMatrix m_raw;

    // Construct from MonoGame matrix
    private Matrix(MGMatrix raw)
    {
        m_raw = raw;
    }

    /// <summary>
    /// Creates a matrix with the given elements in row-major order.
    /// </summary>
    public Matrix(
        float m11, float m12, float m13, float m14,
        float m21, float m22, float m23, float m24,
        float m31, float m32, float m33, float m34,
        float m41, float m42, float m43, float m44)
    {
        m_raw = new MGMatrix(
            m11, m12, m13, m14,
            m21, m22, m23, m24,
            m31, m32, m33, m34,
            m41, m42, m43, m44);
    }

    // Expose matrix elements as properties
    public float m11 { get => m_raw.M11; set => m_raw.M11 = value; }
    public float m12 { get => m_raw.M12; set => m_raw.M12 = value; }
    public float m13 { get => m_raw.M13; set => m_raw.M13 = value; }
    public float m14 { get => m_raw.M14; set => m_raw.M14 = value; }
    public float m21 { get => m_raw.M21; set => m_raw.M21 = value; }
    public float m22 { get => m_raw.M22; set => m_raw.M22 = value; }
    public float m23 { get => m_raw.M23; set => m_raw.M23 = value; }
    public float m24 { get => m_raw.M24; set => m_raw.M24 = value; }
    public float m31 { get => m_raw.M31; set => m_raw.M31 = value; }
    public float m32 { get => m_raw.M32; set => m_raw.M32 = value; }
    public float m33 { get => m_raw.M33; set => m_raw.M33 = value; }
    public float m34 { get => m_raw.M34; set => m_raw.M34 = value; }
    public float m41 { get => m_raw.M41; set => m_raw.M41 = value; }
    public float m42 { get => m_raw.M42; set => m_raw.M42 = value; }
    public float m43 { get => m_raw.M43; set => m_raw.M43 = value; }
    public float m44 { get => m_raw.M44; set => m_raw.M44 = value; }

    /// <summary>
    /// Returns the identity matrix.
    /// </summary>
    public static Matrix identity => new Matrix(MGMatrix.Identity);

    /// <summary>
    /// Creates a translation matrix.
    /// </summary>
    public static Matrix CreateTranslation(float x, float y, float z)
    {
        return new Matrix(MGMatrix.CreateTranslation(x, y, z));
    }

    public static Matrix CreateTranslation(Vector3 position)
    {
        return new Matrix(MGMatrix.CreateTranslation(position));
    }

    /// <summary>
    /// Creates a scaling matrix.
    /// </summary>
    public static Matrix CreateScale(float scale)
    {
        return new Matrix(MGMatrix.CreateScale(scale));
    }

    public static Matrix CreateScale(float x, float y, float z)
    {
        return new Matrix(MGMatrix.CreateScale(x, y, z));
    }

    public static Matrix CreateScale(Vector3 scales)
    {
        return new Matrix(MGMatrix.CreateScale(scales));
    }

    /// <summary>
    /// Creates a rotation matrix around the X axis.
    /// </summary>
    public static Matrix CreateRotationX(float radians)
    {
        return new Matrix(MGMatrix.CreateRotationX(radians));
    }

    /// <summary>
    /// Creates a rotation matrix around the Y axis.
    /// </summary>
    public static Matrix CreateRotationY(float radians)
    {
        return new Matrix(MGMatrix.CreateRotationY(radians));
    }

    /// <summary>
    /// Creates a rotation matrix around the Z axis.
    /// </summary>
    public static Matrix CreateRotationZ(float radians)
    {
        return new Matrix(MGMatrix.CreateRotationZ(radians));
    }

    /// <summary>
    /// Creates a rotation matrix from a quaternion.
    /// </summary>
    public static Matrix CreateFromQuaternion(Quaternion quaternion)
    {
        return new Matrix(MGMatrix.CreateFromQuaternion(quaternion));
    }

    /// <summary>
    /// Creates a perspective field of view matrix.
    /// </summary>
    public static Matrix CreatePerspectiveFieldOfView(float fov, float aspectRatio, float nearPlane, float farPlane)
    {
        return new Matrix(MGMatrix.CreatePerspectiveFieldOfView(fov, aspectRatio, nearPlane, farPlane));
    }

    /// <summary>
    /// Creates an orthographic projection matrix.
    /// </summary>
    public static Matrix CreateOrthographic(float width, float height, float nearPlane, float farPlane)
    {
        return new Matrix(MGMatrix.CreateOrthographic(width, height, nearPlane, farPlane));
    }

    /// <summary>
    /// Creates a look-at view matrix.
    /// </summary>
    public static Matrix CreateLookAt(Vector3 cameraPosition, Vector3 cameraTarget, Vector3 cameraUpVector)
    {
        return new Matrix(MGMatrix.CreateLookAt(cameraPosition, cameraTarget, cameraUpVector));
    }

    /// <summary>
    /// Multiplies two matrices.
    /// </summary>
    public static Matrix Multiply(Matrix a, Matrix b)
    {
        return new Matrix(MGMatrix.Multiply(a.m_raw, b.m_raw));
    }

    public static Matrix operator *(Matrix a, Matrix b)
    {
        return Multiply(a, b);
    }

    public static bool operator ==(Matrix a, Matrix b) => a.m_raw == b.m_raw;
    public static bool operator !=(Matrix a, Matrix b) => a.m_raw != b.m_raw;

    public override bool Equals(object? obj)
    {
        if (obj is Matrix other)
            return m_raw.Equals(other.m_raw);
        return false;
    }

    public bool Equals(Matrix other) => m_raw.Equals(other.m_raw);

    public override int GetHashCode() => m_raw.GetHashCode();

    public override string ToString() => m_raw.ToString();

    // Implicit conversion to/from MonoGame Matrix
    public static implicit operator MGMatrix(Matrix m) => m.m_raw;
    public static implicit operator Matrix(MGMatrix m) => new Matrix(m);
}