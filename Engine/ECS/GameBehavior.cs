namespace Engine.ECS;

/// <summary>
/// Abstract class for game behaviors.
/// GameBehaviors are GameComponents whose active states can be set.
/// </summary>
public abstract class GameBehavior : GameComponent
{
    public new bool IsActive
    {
        get => base.IsActive;
        private set => base.IsActive = value;
    }
    
    /// <summary>
    /// This is called when the behavior is set to active.
    /// </summary>
    protected virtual void OnEnable() { }
    
    /// <summary>
    /// This is called when the behavior is set to inactive.
    /// </summary>
    protected virtual void OnDisable() { }

    /// <summary>
    /// Set the active state of the behavior.
    /// </summary>
    /// <param name="active">The active state</param>
    public void SetActive(bool active)
    {
        if (IsActive == active) return;

        IsActive = active;

        if (active) {OnEnable();}
        else {OnDisable();}
    }
}