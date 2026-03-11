using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace Task1;

public static class Program
{
    public static void Main()
    {
        NativeWindowSettings nativeWinSettings = new NativeWindowSettings()
        {
            ClientSize = new Vector2i(800, 800),
            Location = new Vector2i(30, 30),
            WindowBorder = WindowBorder.Resizable,
            WindowState = WindowState.Normal,
            Title = "Icosahedron",
            Flags = ContextFlags.Default,
            Profile = ContextProfile.Compatability,
        };

        Window game = new Window(GameWindowSettings.Default, nativeWinSettings);
        
        game.Run();
    }
}