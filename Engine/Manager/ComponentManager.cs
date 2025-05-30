using Engine.Core;

namespace Engine.Manager;

/// <summary>
/// Manages components by entity and updates components ordered by tag.
/// </summary>
public class ComponentManager
{
    private static readonly ComponentTag[] OrderedTags = Enum.GetValues<ComponentTag>();

    private readonly Dictionary<ComponentTag, List<GameComponent>> _componentsByTag = new();
    private readonly Dictionary<Guid, Dictionary<Type, GameComponent>> _componentsByEntity = new();

    public ComponentManager()
    {
        foreach (var tag in Enum.GetValues<ComponentTag>())
        {
            _componentsByTag[tag] = [];
        }
    }

    /// <summary>
    /// Adds a component of type T to the entity if it doesn't exist.
    /// Returns the existing or new component instance.
    /// </summary>
    public T Add<T>(Guid entityId) where T : GameComponent, new()
    {
        // Initialize the List for a new entity
        if (!_componentsByEntity.TryGetValue(entityId, out var entityComponents))
        {
            entityComponents = new Dictionary<Type, GameComponent>();
            _componentsByEntity[entityId] = entityComponents;
        }

        var type = typeof(T);

        // Already exists, return it directly
        if (entityComponents.TryGetValue(type, out var existingComponent))
        {
            return (T)existingComponent;
        }

        // Create new component instance
        var component = new T();
        component.Initialize(entityId);
        entityComponents[type] = component;
        _componentsByTag[component.OrderTag].Add(component);

        return component;
    }

    /// <summary>
    /// Removes a component of type T from the entity.
    /// </summary>
    public void Remove<T>(Guid entityId) where T : GameComponent
    {
        if (!_componentsByEntity.TryGetValue(entityId, out var entityComponents))
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
        _componentsByTag[component.OrderTag].Remove(component);

        if (entityComponents.Count == 0)
        {
            _componentsByEntity.Remove(entityId);
        }
    }

    /// <summary>
    /// Gets the component of type T for the entity. Returns null if not found.
    /// </summary>
    public T? Get<T>(Guid entityId) where T : GameComponent
    {
        if (!_componentsByEntity.TryGetValue(entityId, out var entityComponents))
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
    /// Checks if entity has component of type T.
    /// </summary>
    public bool Has<T>(Guid entityId) where T : GameComponent
    {
        return _componentsByEntity.TryGetValue(entityId, out var entityComponents)
            && entityComponents.ContainsKey(typeof(T));
    }

    /// <summary>
    /// Updates all components ordered by ComponentTag enum.
    /// </summary>
    public void UpdateAll()
    {
        foreach (var tag in OrderedTags)
        {
            foreach (var component in _componentsByTag[tag])
            {
                component.Update();
            }
        }
    }

    /// <summary>
    /// Wakes up all components ordered by ComponentTag enum. This should be called after all the loading processed
    /// are done.
    /// </summary>
    public void WakeAll()
    {
        foreach (var tag in OrderedTags)
        {
            foreach (var component in _componentsByTag[tag])
            {
                component.Awake();
            }
        }
    }  
    
    /// <summary>
    /// Starts all components ordered by ComponentTag enum. This should be called in the very first frame.
    /// </summary>
    public void StartAll()
    {
        foreach (var tag in OrderedTags)
        {
            foreach (var component in _componentsByTag[tag])
            {
                component.Awake();
            }
        }
    }
}
