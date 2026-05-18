namespace Task1;

public static class Program
{
    public static void Main()
    {
        try
        {
            using PhongScene phongScene = new PhongScene();
            phongScene.Run();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fatal error: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
        }
    }
}