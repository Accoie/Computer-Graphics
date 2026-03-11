using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace Task1;

public class Program
{
    static void Main()
    {
        GameWindowSettings? gameWindowSettings = GameWindowSettings.Default;

        NativeWindowSettings nativeWindowSettings = new NativeWindowSettings
        {
            ClientSize = new Vector2i( 1000, 800 ),
            Title = "Bezier Curve",
            APIVersion = new Version( 2, 1 ),
            Profile = ContextProfile.Any,
            Flags = ContextFlags.Default
        };

        using BezierGameWindow gameWindow = new( gameWindowSettings, nativeWindowSettings );
        gameWindow.Run();
    }
}