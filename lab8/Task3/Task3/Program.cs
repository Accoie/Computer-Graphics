using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace Task3;

public static class Program
{
    public static void Main()
    {
        NativeWindowSettings nativeWindowSettings = new NativeWindowSettings
        {
            ClientSize = new OpenTK.Mathematics.Vector2i(982, 853),
            Title = "Pyramid",
            Flags = ContextFlags.Default,
            API = ContextAPI.OpenGL,
            APIVersion = new Version(3, 3, 0, 0),
            Profile = ContextProfile.Core,
        };

        GameWindowSettings? gameWindowSettings = GameWindowSettings.Default;

        using MainWindow window = new MainWindow(gameWindowSettings, nativeWindowSettings);
        window.Run();
    }
}