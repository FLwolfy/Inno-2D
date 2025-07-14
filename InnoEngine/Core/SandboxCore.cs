using InnoEngine.ECS;

namespace InnoEngine.Core;

/// <summary>
/// This is the base class for all sandbox shells. This is used for the runtime implementation
/// of the game engine.
/// </summary>
public abstract class SandboxCore : EngineCore
{
    public sealed override void Step()
    {
        // TODO: Physics Update

        // Regular Update
        SceneManager.GetActiveScene()?.Update();
    }
}