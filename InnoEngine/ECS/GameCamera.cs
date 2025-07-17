using InnoBase;

namespace InnoEngine.ECS;

public abstract class GameCamera : GameComponent
{
    public sealed override ComponentTag orderTag => ComponentTag.Camera;

    private bool m_dirtyMatrix = true;

    private Matrix m_cachedViewMatrix;
    private Matrix m_cachedProjectionMatrix;
    private Rect m_cachedViewRect;

    public bool isMainCamera
    {
        get => gameObject.scene.GetCameraManager().mainCamera == this;
        set { gameObject.scene.GetCameraManager().SetMainCamera(value ? this : null); }
    }

    public Matrix viewMatrix
    {
        get
        {
            EnsureMatrix();
            return m_cachedViewMatrix;
        }
    }

    public Matrix projectionMatrix
    {
        get
        {
            EnsureMatrix();
            return m_cachedProjectionMatrix;
        }
    }

    public Rect viewRect
    {
        get
        {
            EnsureMatrix();
            return m_cachedViewRect;
        }
    }

    protected void MarkDirty()
    {
        m_dirtyMatrix = true;
    }

    private void EnsureMatrix()
    {
        if (m_dirtyMatrix)
        {
            RebuildMatrix(out m_cachedViewMatrix, out m_cachedProjectionMatrix, out m_cachedViewRect);
            m_dirtyMatrix = false;
        }
    }

    protected abstract void RebuildMatrix(out Matrix view, out Matrix projection, out Rect viewRect);

    public override void Awake()
    {
        transform.OnChanged += MarkDirty;
    }

    public override void OnDetach()
    {
        CameraManager cameraManager = gameObject.scene.GetCameraManager();
        if (cameraManager.mainCamera == this)
        {
            cameraManager.SetMainCamera(null);
        }

        transform.OnChanged -= MarkDirty;
    }
}