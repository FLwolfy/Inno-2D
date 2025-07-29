using InnoEngine.Serialization;

namespace InnoEngine.ECS;

/// <summary>
/// Represents a scene that contains and manages GameObjects.
/// </summary>
public class GameScene : Serializable
{
    public readonly Guid id = Guid.NewGuid();
    public string name = "GameScene";
    
    private bool m_isRunning;
    private bool m_isUpdating;
    
    private readonly List<GameObject> m_gameObjects = [];
    private readonly List<GameObject> m_pendingGameObjectRemoves = [];
    private readonly ComponentManager m_componentManager = new();
    private readonly CameraManager m_cameraManager = new();

    internal GameScene(){}
    
    internal GameScene(string name)
    {
        this.name = name;
    }
    
    /// <summary>
    /// Gets the component manager of the current game scene.
    /// </summary>
    internal ComponentManager GetComponentManager() => m_componentManager;
        
    /// <summary>
    /// Gets the camera manager of the current game scene.
    /// </summary>
    internal CameraManager GetCameraManager() => m_cameraManager;

    /// <summary>
    /// Registers a GameObject to this scene.
    /// </summary>
    public void RegisterGameObject(GameObject obj)
    {
        if (m_gameObjects.Contains(obj)) { return; }
        m_gameObjects.Add(obj);
    }

    /// <summary>
    /// Unregisters a GameObject from this scene.
    /// </summary>
    public void UnregisterGameObject(GameObject obj)
    {
        var components = m_componentManager.GetAll(obj.id);
        foreach (var comp in components)
        {
            m_componentManager.Remove(obj.id, comp);
        }

        if (m_isRunning || m_isUpdating)
        {
            m_pendingGameObjectRemoves.Add(obj);
        }
        else
        {
            m_gameObjects.Remove(obj);
        }
    }

    /// <summary>
    /// Get a gameobject with its uuid.
    /// </summary>
    public GameObject? FindGameObject(Guid uid)
    {
        return m_gameObjects.Find(obj => obj.id == uid);
    }
    
    /// <summary>
    /// Get a gameobject with its name.
    /// </summary>
    public GameObject? FindGameObject(string objName)
    {
        return m_gameObjects.Find(obj => obj.name == objName);
    }

    /// <summary>
    /// Gets all GameObjects in this scene.
    /// </summary>
    public IReadOnlyList<GameObject> GetAllGameObjects()
    {
        return m_gameObjects;
    }

    /// <summary>
    /// Gets all GameObjects that have no parent in this scene.
    /// </summary>
    public IReadOnlyList<GameObject> GetAllRootGameObjects()
    {
        return m_gameObjects.Where(go => go.transform.parent == null).ToList();
    }

    /// <summary>
    /// Called when the game started.
    /// </summary>
    internal void Start()
    {
        m_componentManager.WakeAll();
        m_componentManager.BeginRuntime();
        m_isRunning = true;
    }

    /// <summary>
    /// Update the GameScene and its gameObjects and components.
    /// </summary>
    internal void Update()
    {
        // Updates
        m_isUpdating = true;
        m_componentManager.UpdateAll();
        m_isUpdating = false;
        
        // Remove objects
        foreach (var gameObject in m_pendingGameObjectRemoves) { m_gameObjects.Remove(gameObject); }
        m_pendingGameObjectRemoves.Clear();
    }
}