using InnoEngine.ECS.Component;
using InnoEngine.Serialization;

namespace InnoEngine.ECS;

/// <summary>
/// Represents an entity in the scene. It holds an ID and allows component operations.
/// </summary>
public class GameObject : Serializable
{
    public readonly Guid id = Guid.NewGuid();
    public readonly Transform transform;
    public readonly GameScene scene;
    
    public string name = "GameObject";

    public GameObject()
    {
        GameScene? activeScene = SceneManager.GetActiveScene();
        if (activeScene == null)
        {
            throw new InvalidOperationException("Can not attach GameObject to a null scene.");
        }

        scene = activeScene;
        scene.RegisterGameObject(this);
        
        transform = AddComponent<Transform>();
    }
    
    public GameObject(string name) : this()
    {
        this.name = name;
    }

    public GameObject(GameScene scene)
    {
        this.scene = scene;
        scene.RegisterGameObject(this);
        
        transform = AddComponent<Transform>();
    }

    public GameObject(GameScene scene, string name) : this(scene)
    {
        this.name = name;
    }

    /// <summary>
    /// Adds a component of type T to this GameObject.
    /// </summary>
    public T AddComponent<T>() where T : GameComponent, new()
    {
        return scene.GetComponentManager().Add<T>(this);
    }

    /// <summary>
    /// Gets a component of type T from this GameObject.
    /// </summary>
    public T? GetComponent<T>() where T : GameComponent
    {
        return scene.GetComponentManager().Get<T>(id);
    }
    
    /// <summary>
    /// Gets all Game Components of this gameObject.
    /// </summary>
    public IReadOnlyList<GameComponent> GetAllComponents()
    {
        return scene.GetComponentManager().GetAll(id).ToList();
    }

    /// <summary>
    /// Checks whether the GameObject has a component of type T.
    /// </summary>
    public bool HasComponent<T>() where T : GameComponent
    {
        return scene.GetComponentManager().Has<T>(id);
    }

    /// <summary>
    /// Removes a component of type T from this GameObject.
    /// </summary>
    public void RemoveComponent<T>() where T : GameComponent
    {
        if (typeof(T) == typeof(Transform))
        {
            // TODO: Add warning here.
            return;
        }
        scene.GetComponentManager().Remove<T>(id);
    }

    /// <summary>
    /// Remove the component from this gameObject.
    /// </summary>
    public void RemoveComponent(GameComponent component)
    {
        if (component is Transform)
        {
            // TODO: Add warning here.
            return;
        }
        scene.GetComponentManager().Remove(id, component);
    }
}