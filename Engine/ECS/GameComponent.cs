namespace Engine.ECS;

/// <summary>
/// Abstract class for game components.
/// </summary>
public abstract class GameComponent
{
    public Guid EntityId { get; private set; }
    public bool IsActive { get; protected set; } = true;

    public bool HasAwakened { get; internal set; }
    public bool HasStarted { get; internal set; }

    /// <summary>
    /// Initialize the gameComponent to bind it with its entity.
    /// </summary>
    /// <param name="entityId"></param>
    public void Initialize(Guid entityId)
    {
        EntityId = entityId;
    }
    
    /// <summary>
    /// This is the component Tag of the game component. This is used for indicating the component's update order.
    /// </summary>
    public abstract ComponentTag OrderTag { get; }

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
}