using InnoEngine.ECS;
using Microsoft.Xna.Framework;

namespace InnoEngine.Core;

/// <summary>
/// This is the base class for all sandbox shells. This is used for the runtime implementation
/// of the game engine.
/// </summary>
public abstract class SandboxShell : GameShell
{
    public sealed override void Step(GameTime gameTime)
    {
        SceneManager.GetActiveScene()?.Update();
    }
}