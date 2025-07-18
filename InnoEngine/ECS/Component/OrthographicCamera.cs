using InnoBase;

namespace InnoEngine.ECS.Component;

/// <summary>
/// Orthographic (2D) camera component.
/// </summary>
public class OrthographicCamera : GameCamera
{
    private const float c_near = -1f;
    private const float c_far = 1f;

    private float m_size = 10f;
    private float m_aspectRatio = 1.7777f;

    public float size
    {
        get => m_size;
        set
        {
            if (m_size != value)
            {
                m_size = value;
                MarkDirty();
            }
        }
    }

    public float aspectRatio
    {
        get => m_aspectRatio;
        set
        {
            if (MathF.Abs(m_aspectRatio - value) > 0.0001f)
            {
                m_aspectRatio = value;
                MarkDirty();
            }
        }
    }

    protected override void RebuildMatrix(out Matrix view, out Matrix projection, out Rect viewRect)
    {
        Vector2 cameraPos = new Vector2(transform.worldPosition.x, transform.worldPosition.y);
        float rotationZ = transform.worldRotation.ToEulerAnglesZYX().z;

        projection = CalculateProjectionMatrix();
        view = CalculateViewMatrix(cameraPos, rotationZ);
        viewRect = CalculateViewRect(cameraPos, rotationZ);
    }

    private Matrix CalculateProjectionMatrix()
    {
        float halfHeight = m_size * 0.5f;
        float halfWidth = halfHeight * m_aspectRatio;

        return Matrix.CreateOrthographic(
            width: halfWidth * 2,
            height: halfHeight * 2,
            c_near,
            c_far
        );
    }

    private Matrix CalculateViewMatrix(Vector2 position, float rotationZ)
    {
        Matrix rotation = Matrix.CreateRotationZ(-rotationZ);
        Matrix translation = Matrix.CreateTranslation(-position.x, -position.y, 0f);
        return rotation * translation;
    }

    private Rect CalculateViewRect(Vector2 position, float rotationZ)
    {
        float halfHeight = m_size * 0.5f;
        float halfWidth = halfHeight * m_aspectRatio;

        Vector2[] corners = new Vector2[]
        {
            new Vector2(-halfWidth, -halfHeight),
            new Vector2(halfWidth, -halfHeight),
            new Vector2(halfWidth, halfHeight),
            new Vector2(-halfWidth, halfHeight)
        };

        float cos = MathF.Cos(rotationZ);
        float sin = MathF.Sin(rotationZ);

        for (int i = 0; i < corners.Length; i++)
        {
            float x = corners[i].x;
            float y = corners[i].y;

            corners[i] = new Vector2(
                x * cos - y * sin,
                x * sin + y * cos
            ) + position;
        }

        float minX = corners.Min(c => c.x);
        float maxX = corners.Max(c => c.x);
        float minY = corners.Min(c => c.y);
        float maxY = corners.Max(c => c.y);

        return new Rect(
            x: (int)minX,
            y: (int)minY,
            width: (int)(maxX - minX),
            height: (int)(maxY - minY)
        );
    }
}