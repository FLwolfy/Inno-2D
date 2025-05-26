using Engine;

namespace Sandbox;

/*
 * This is the entry point of the game runtime.
 */
class Program
{
    /** @brief Entry point of the test sandbox application. */
    static void Main()
    {
        using var game = new GameShell();
        game.Run();
    }
}