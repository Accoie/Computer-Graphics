using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace  Task2
{
    public static class Program
    {
        public static void Main()
        {
            NativeWindowSettings nativeWinSettings = new()
            {
                ClientSize = new Vector2i(800, 800),
                Location = new Vector2i(30, 30),
                WindowBorder = WindowBorder.Resizable,
                WindowState = WindowState.Normal,
                Title = "MobiusStrip",
                Flags = ContextFlags.Default,
                APIVersion = new Version(3, 3),
                Profile = ContextProfile.Compatability,
                API = ContextAPI.OpenGL,
                NumberOfSamples = 0
            };


            Window game = new(GameWindowSettings.Default, nativeWinSettings);
            game.Run();

        }
    }
}