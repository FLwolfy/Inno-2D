using InnoEngine.ECS.Component;
using InnoEngine.Serialization;

namespace InnoEngine.ECS;

/// <summary>
/// Abstract class for game components.
/// </summary>
public abstract class GameComponent : Serializable, IComparable<GameComponent>
{
    public GameObject gameObject { get; private set; } = null!;
    public Transform transform => gameObject.transform;
    
    public bool isActive { get; protected set; } = true;
    public bool hasAwakened { get; internal set; }
    public bool hasStarted { get; internal set; }
    
    public int CompareTo(GameComponent? other) => other == null ? 1 : orderTag.CompareTo(other.orderTag);
    
    /// <summary>
    /// Initialize the gameComponent to bind it with its entity.
    /// </summary>
    public void Initialize(GameObject obj)
    {
        gameObject = obj;
    }
    
    /// <summary>
    /// This is the component Tag of the game component. This is used for indicating the component's update order.
    /// </summary>
    public abstract ComponentTag orderTag { get; }
    
    /// <summary>
    /// Ways to sort a list of GameComponents, using orderTag as default.
    /// </summary>
    protected virtual int Compare(GameComponent other)
    {
        return orderTag.CompareTo(other.orderTag);
    }

    /// <summary>
    /// Called when the component is first initialized.
    /// </summary>
    public virtual void Awake() {}

    /// <summary>
    /// Called once before the first update.
    /// </summary>
    public virtual void Start() {}

    /// <summary>
    /// Called every frame to update component logic.
    /// </summary>
    public virtual void Update() {}

    /// <summary>
    /// Called when the component is destroyed or removed.
    /// </summary>
    public virtual void OnDetach() {}
    
    /// <summary>
    /// Adds a component of type T to its GameObject.
    /// </summary>
    protected T AddComponent<T>() where T : GameComponent, new()
    {
        return gameObject.AddComponent<T>();
    }

    /// <summary>
    /// Gets a component of type T from its GameObject.
    /// </summary>
    protected T? GetComponent<T>() where T : GameComponent
    {
        return gameObject.GetComponent<T>();
    }

    /// <summary>
    /// Checks whether its GameObject has a component of type T.
    /// </summary>
    protected bool HasComponent<T>() where T : GameComponent
    {
        return gameObject.HasComponent<T>();
    }

    /// <summary>
    /// Removes a component of type T from its GameObject.
    /// </summary>
    protected void RemoveComponent<T>() where T : GameComponent
    {
        gameObject.RemoveComponent<T>();
    }
}