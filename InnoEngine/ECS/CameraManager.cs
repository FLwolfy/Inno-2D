namespace InnoEngine.ECS;

internal class CameraManager
{
    private GameCamera? m_mainCamera;

    public GameCamera? mainCamera => m_mainCamera;

    public void SetMainCamera(GameCamera? camera)
    {
        m_mainCamera = camera;
        
        // TODO: add switching logics here
    }
}