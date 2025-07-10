using Engine.Core;

namespace Sandbox;

class Program
{
    /// <summary>
    /// Entry point of the test sandbox application.
    /// </summary>
    static void Main()
    {
        using var game = new GameShell();
        game.Run();
    }
}