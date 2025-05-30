using Engine;

namespace Sandbox;

class Program
{
    /** @brief Entry point of the test sandbox application. */
    static void Main()
    {
        using var game = new GameShell();
        game.Run();
    }
}