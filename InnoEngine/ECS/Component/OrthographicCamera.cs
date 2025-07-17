using InnoBase;

namespace InnoEngine.ECS.Component;

/// <summary>
/// Orthographic (2D) camera component.
/// </summary>
public class OrthographicCamera : GameCamera
{
    private float m_size = 10f; // 垂直视野大小，水平视野由宽高比决定
    private float m_aspectRatio = 1.7777f;     // 默认 16:9

    /// <summary>
    /// 垂直方向的视图大小（摄像机高度）。修改后会自动更新矩阵。
    /// </summary>
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

    /// <summary>
    /// 屏幕宽高比。修改后会自动更新矩阵。
    /// </summary>
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
        // TODO: FIX THIS
        Vector2 position = new Vector2(transform.worldPosition.x, transform.worldPosition.y);
        float halfHeight = m_size * 0.5f;
        float halfWidth = halfHeight * m_aspectRatio;

        // 构造正交投影矩阵（无旋转，中心对称）
        projection = Matrix.CreateOrthographic(halfWidth * 2, halfHeight * 2, -1f, 1f);

        // 计算视图矩阵，先旋转再平移（逆变换）
        float rotationZ = transform.worldRotation.ToEulerAnglesZYX().z;
        Matrix rotation = Matrix.CreateRotationZ(-rotationZ);
        Matrix translation = Matrix.CreateTranslation(-position.x, -position.y, 0f);

        view = rotation * translation;

        // 计算旋转后的viewRect (轴对齐包围盒)
        // 1. 定义四个视口角点相对于摄像机中心的向量
        Vector2[] corners = new Vector2[]
        {
            new Vector2(-halfWidth, -halfHeight),
            new Vector2(halfWidth, -halfHeight),
            new Vector2(halfWidth, halfHeight),
            new Vector2(-halfWidth, halfHeight)
        };

        // 2. 旋转四个角点
        float cos = MathF.Cos(rotationZ);
        float sin = MathF.Sin(rotationZ);
        for(int i=0; i<corners.Length; i++)
        {
            float x = corners[i].x;
            float y = corners[i].y;
            corners[i] = new Vector2(
                x * cos - y * sin,
                x * sin + y * cos
            ) + position;
        }

        // 3. 计算包围盒
        float minX = corners.Min(c => c.x);
        float maxX = corners.Max(c => c.x);
        float minY = corners.Min(c => c.y);
        float maxY = corners.Max(c => c.y);

        viewRect = new Rect((int)minX, (int)minY, (int)(maxX - minX), (int)(maxY - minY));
    }
}