using InnoEngine.ECS.Component;

namespace InnoEngine.ECS;

/// <summary>
/// Represents an entity in the scene. It holds an ID and allows component operations.
/// </summary>
public class GameObject
{
    public readonly Guid id = Guid.NewGuid();
    public readonly Transform transform;
    
    public string name = "GameObject";
    private readonly GameScene m_scene;

    public GameObject()
    {
        m_scene = SceneManager.GetActiveScene();
        m_scene.RegisterGameObject(this);
        
        transform = AddComponent<Transform>();
    }
    
    public GameObject(string name)
    {
        m_scene = SceneManager.GetActiveScene();
        m_scene.RegisterGameObject(this);
        this.name = name;
        
        transform = AddComponent<Transform>();
    }

    public GameObject(GameScene scene)
    {
        m_scene = scene;
        scene.RegisterGameObject(this);
        
        transform = AddComponent<Transform>();
    }

    public GameObject(GameScene scene, string name)
    {
        m_scene = scene;
        scene.RegisterGameObject(this);
        this.name = name;
        
        transform = AddComponent<Transform>();
    }

    /// <summary>
    /// Adds a component of type T to this GameObject.
    /// </summary>
    public T AddComponent<T>() where T : GameComponent, new()
    {
        return m_scene.GetComponentManager().Add<T>(this);
    }

    /// <summary>
    /// Gets a component of type T from this GameObject.
    /// </summary>
    public T? GetComponent<T>() where T : GameComponent
    {
        return m_scene.GetComponentManager().Get<T>(id);
    }

    /// <summary>
    /// Checks whether the GameObject has a component of type T.
    /// </summary>
    public bool HasComponent<T>() where T : GameComponent
    {
        return m_scene.GetComponentManager().Has<T>(id);
    }

    /// <summary>
    /// Removes a component of type T from this GameObject.
    /// </summary>
    public void RemoveComponent<T>() where T : GameComponent
    {
        m_scene.GetComponentManager().Remove<T>(id);
    }
}