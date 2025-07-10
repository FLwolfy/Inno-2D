using Microsoft.Xna.Framework;

namespace Engine.Extension;

public static class QuaternionExtension
{
    /// <summary>
    /// Converts a quaternion to Euler angles in XYZ order (pitch X, yaw Y, roll Z).
    /// The result is in radians. This order is commonly used in math and XNA/MonoGame.
    /// </summary>
    public static Vector3 ToEulerAnglesXYZ(this Quaternion q)
    {
        double sinrCosp = 2 * (q.W * q.X + q.Y * q.Z);
        double cosrCosp = 1 - 2 * (q.X * q.X + q.Y * q.Y);
        float x = (float)Math.Atan2(sinrCosp, cosrCosp);

        double sinp = 2 * (q.W * q.Y - q.Z * q.X);
        float y;
        if (Math.Abs(sinp) >= 1)
            y = (float)(Math.CopySign(Math.PI / 2, sinp)); // Use 90 degrees if out of range
        else
            y = (float)Math.Asin(sinp);

        double sinyCosp = 2 * (q.W * q.Z + q.X * q.Y);
        double cosyCosp = 1 - 2 * (q.Y * q.Y + q.Z * q.Z);
        float z = (float)Math.Atan2(sinyCosp, cosyCosp);

        return new Vector3(x, y, z);
    }

    /// <summary>
    /// Converts a quaternion to Euler angles in ZYX order (roll Z, yaw Y, pitch X).
    /// The result is in radians. This order matches Unity's internal rotation system.
    /// </summary>
    public static Vector3 ToEulerAnglesZYX(this Quaternion q)
    {
        // Z-axis rotation (roll)
        double sinrCosp = 2 * (q.W * q.Z + q.X * q.Y);
        double cosrCosp = 1 - 2 * (q.Y * q.Y + q.Z * q.Z);
        float z = (float)Math.Atan2(sinrCosp, cosrCosp);

        // Y-axis rotation (yaw)
        double sinp = 2 * (q.W * q.Y - q.Z * q.X);
        float y;
        if (Math.Abs(sinp) >= 1)
            y = (float)(Math.CopySign(Math.PI / 2, sinp)); // Use 90 degrees if out of range
        else
            y = (float)Math.Asin(sinp);

        // X-axis rotation (pitch)
        double sinyCosp = 2 * (q.W * q.X + q.Y * q.Z);
        double cosyCosp = 1 - 2 * (q.X * q.X + q.Y * q.Y);
        float x = (float)Math.Atan2(sinyCosp, cosyCosp);

        return new Vector3(x, y, z);
    }

    /// <summary>
    /// Converts Euler angles to a quaternion using XYZ order.
    /// The input angles are in radians and are applied in X → Y → Z order.
    /// </summary>
    public static Quaternion FromEulerAnglesXYZ(Vector3 euler)
    {
        float cx = (float)Math.Cos(euler.X * 0.5);
        float sx = (float)Math.Sin(euler.X * 0.5);
        float cy = (float)Math.Cos(euler.Y * 0.5);
        float sy = (float)Math.Sin(euler.Y * 0.5);
        float cz = (float)Math.Cos(euler.Z * 0.5);
        float sz = (float)Math.Sin(euler.Z * 0.5);

        return new Quaternion(
            sx * cy * cz + cx * sy * sz, // X
            cx * sy * cz - sx * cy * sz, // Y
            cx * cy * sz + sx * sy * cz, // Z
            cx * cy * cz - sx * sy * sz  // W
        );
    }

    /// <summary>
    /// Converts Euler angles to a quaternion using ZYX order.
    /// The input angles are in radians and are applied in Z → Y → X order.
    /// This order is used internally by Unity.
    /// </summary>
    public static Quaternion FromEulerAnglesZYX(Vector3 euler)
    {
        float cz = (float)Math.Cos(euler.Z * 0.5);
        float sz = (float)Math.Sin(euler.Z * 0.5);
        float cy = (float)Math.Cos(euler.Y * 0.5);
        float sy = (float)Math.Sin(euler.Y * 0.5);
        float cx = (float)Math.Cos(euler.X * 0.5);
        float sx = (float)Math.Sin(euler.X * 0.5);

        return new Quaternion(
            sx * cy * cz - cx * sy * sz, // X
            cx * sy * cz + sx * cy * sz, // Y
            cx * cy * sz - sx * sy * cz, // Z
            cx * cy * cz + sx * sy * sz  // W
        );
    }
}

