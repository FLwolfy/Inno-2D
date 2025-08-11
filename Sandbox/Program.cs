namespace Sandbox;

internal static class Program
{
    /// <summary>
    /// Entry point of the test sandbox application.
    /// </summary>
    public static void Main()
    {
        // var game = new TestGame();
        // game.Run();
        
        var testInternal = new TestInternal();
        testInternal.Render();
        
    }
}

internal class TestInternal
{
    public void Render()
    {
        Console.WriteLine("Render");
    }
}