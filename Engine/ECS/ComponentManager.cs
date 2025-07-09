namespace Engine.ECS;

/// <summary>
/// Manages components by entity and updates components ordered by tag.
/// </summary>
public class ComponentManager
{
    private bool m_isRunning;
    private bool m_isUpdating;
    
    private static readonly ComponentTag[] ORDERED_TAGS = Enum.GetValues<ComponentTag>();

    private readonly Dictionary<ComponentTag, HashSet<GameComponent>> m_componentsByTag = new();
    private readonly Dictionary<Guid, Dictionary<Type, GameComponent>> m_componentsByEntity = new();
    
    private readonly List<Action> m_pendingAddRemoves = [];
    private readonly List<GameComponent> m_pendingStart = [];
    
    public ComponentManager()
    {
        foreach (var tag in Enum.GetValues<ComponentTag>())
        {
            m_componentsByTag[tag] = [];
        }
    }
    
    /// <summary>
    /// Tell the manager the game is running.
    /// </summary>
    public void BeginRuntime()
    {
        m_isRunning = true;
    }
    
    /// <summary>
    /// Adds a component of type T to the entity if it doesn't exist.
    /// Returns the existing or new component instance.
    /// </summary>
    public T Add<T>(Guid entityId, GameObject obj) where T : GameComponent, new()
    {
        // Initialize the List for a new entity
        if (!m_componentsByEntity.TryGetValue(entityId, out var entityComponents))
        {
            entityComponents = new Dictionary<Type, GameComponent>();
            m_componentsByEntity[entityId] = entityComponents;
        }

        var type = typeof(T);

        // Already exists, return it directly
        if (entityComponents.TryGetValue(type, out var existingComponent))
        {
            return (T)existingComponent;
        }

        // Create new component instance
        var component = new T();
        component.Initialize(entityId, obj);
        entityComponents[type] = component;
        
        // Add or delay add
        if (m_isUpdating) { m_pendingAddRemoves.Add(() => m_componentsByTag[component.OrderTag].Add(component)); }
        else { m_componentsByTag[component.OrderTag].Add(component); }
        
        // Game already started, execute Awake()
        if (m_isRunning && !component.HasAwakened)
        {
            component.Awake();
            component.HasAwakened = true;
        }

        // Delay Start()
        if (component.IsActive && !component.HasStarted)
        {
            m_pendingStart.Add(component);
        }
        
        return component;
    }

    /// <summary>
    /// Removes a component of type T from the entity.
    /// </summary>
    public void Remove<T>(Guid entityId) where T : GameComponent
    {
        if (m_isUpdating)
        {
            m_pendingAddRemoves.Add(() => Remove<T>(entityId));
            return;
        }
        
        if (!m_componentsByEntity.TryGetValue(entityId, out var entityComponents))
        {
            return;
        }
        
        var type = typeof(T);

        if (!entityComponents.TryGetValue(type, out var component))
        {
            return;
        }
        
        // Remove the component from the dictionaries
        component.OnDetach();
        entityComponents.Remove(type);
        m_componentsByTag[component.OrderTag].Remove(component);

        if (entityComponents.Count == 0)
        {
            m_componentsByEntity.Remove(entityId);
        }
    }
    
    /// <summary>
    /// Removes the specified component instance from the entity.
    /// </summary>
    public void Remove(Guid entityId, GameComponent component)
    {
        if (m_isUpdating)
        {
            m_pendingAddRemoves.Add(() => Remove(entityId, component));
            return;
        }
    
        if (!m_componentsByEntity.TryGetValue(entityId, out var entityComponents))
        {
            return;
        }

        var type = component.GetType();

        if (!entityComponents.TryGetValue(type, out var existingComponent))
        {
            return;
        }
    
        if (!ReferenceEquals(existingComponent, component))
        {
            return;
        }
    
        // Remove the component from the dictionaries
        component.OnDetach();
        entityComponents.Remove(type);
        m_componentsByTag[component.OrderTag].Remove(component);

        if (entityComponents.Count == 0)
        {
            m_componentsByEntity.Remove(entityId);
        }
    }

    /// <summary>
    /// Gets the component of type T for the entity. Returns null if not found.
    /// TODO: IMPROVE EFFICIENCY
    /// </summary>
    public T? Get<T>(Guid entityId) where T : GameComponent
    {
        if (!m_componentsByEntity.TryGetValue(entityId, out var entityComponents))
        {
            return null;
        }
        
        if (entityComponents.TryGetValue(typeof(T), out var component))
        {
            return (T)component;
        }
        
        return null;
    }
    
    /// <summary>
    /// Gets all components of a specific entity.
    /// </summary>
    public IEnumerable<GameComponent> GetAll(Guid entityId)
    {
        if (!m_componentsByEntity.TryGetValue(entityId, out var entityComponents)) {return [];}
        return entityComponents.Values.ToArray();
    }
    
    /// <summary>
    /// Gets all components of a specific type.
    /// </summary>
    public IEnumerable<GameComponent> GetAll<T>() where T : GameComponent
    {
        // TODO complete this
    }
    
    /// <summary>
    /// Checks if entity has component of type T.
    /// </summary>
    public bool Has<T>(Guid entityId) where T : GameComponent
    {
        return m_componentsByEntity.TryGetValue(entityId, out var entityComponents)
            && entityComponents.ContainsKey(typeof(T));
    }

    /// <summary>
    /// Updates all components ordered by ComponentTag enum.
    /// </summary>
    public void UpdateAll()
    {
        // Execute Starts and Updates
        m_isUpdating = true;
        ProcessPendingStarts();
        UpdateComponents();
        m_isUpdating = false;
        
        // Execute pending adds and removes
        ApplyPendingAddRemoves();
    }

    /// <summary>
    /// Wakes up all components ordered by ComponentTag enum. This should be called after all the loading processed
    /// are done.
    /// </summary>
    public void WakeAll()
    {
        foreach (var tag in ORDERED_TAGS)
        {
            foreach (var component in m_componentsByTag[tag])
            {
                if (component.HasAwakened) continue;
                component.Awake();
                component.HasAwakened = true;
            }
        }
    }
    
    private void ProcessPendingStarts()
    {
        foreach (var component in m_pendingStart)
        {
            if (component.IsActive && !component.HasStarted)
            {
                component.Start();
                component.HasStarted = true;
            }
        }
        m_pendingStart.Clear();
    }
    
    private void UpdateComponents()
    {
        foreach (var tag in ORDERED_TAGS)
        {
            foreach (var component in m_componentsByTag[tag])
            {
                if (!component.IsActive) {continue;}

                if (component.HasStarted) { component.Update(); }
                else { m_pendingStart.Add(component); }
            }
        }
    }
    
    private void ApplyPendingAddRemoves()
    {
        foreach (var action in m_pendingAddRemoves) {action();}
        m_pendingAddRemoves.Clear();
    }
}
