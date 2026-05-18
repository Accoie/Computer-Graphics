using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace Task2;

public static class Program
{
    public static void Main()
    {
        var nativeWindowSettings = new NativeWindowSettings
        {
            ClientSize = new Vector2i(982, 853),
            Title = "Task2 - Shadows Visualization",
            Flags = ContextFlags.Default,
            API = ContextAPI.OpenGL,
            APIVersion = new Version(3, 3),
            Profile = ContextProfile.Core,
        };

        var gameWindowSettings = new GameWindowSettings
        {
            UpdateFrequency = 60.0,
        };

        using var app = new Application(gameWindowSettings, nativeWindowSettings);
        app.Run();
    }
}