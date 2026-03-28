namespace Task3;

public static class Program
{
    public static void Main()
    {
        LabyrinthGame game = new LabyrinthGame(1920, 1080, "Labyrinth!");
        game.Run();
    }
}