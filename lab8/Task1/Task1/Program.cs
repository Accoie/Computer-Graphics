namespace Task1;

public static class Program
{
    public static void Main()
    {
        try
        {
            using Game game = new Game();
            game.Run();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fatal error: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
        }
    }
}