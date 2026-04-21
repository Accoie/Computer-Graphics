using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace Task1;

class Program
{
    static void Main(string[] args)
    {
        GameWindowSettings? gameWindowSettings = GameWindowSettings.Default;
        NativeWindowSettings nativeWindowSettings = new NativeWindowSettings
        {
            ClientSize = new Vector2i(1500, 800),
            Title = "OpenTK 3D Application",
            APIVersion = new Version(3, 3),
            Profile = ContextProfile.Core,
            Flags = ContextFlags.ForwardCompatible
        };
        using ChessGame chessGame = new ChessGame(gameWindowSettings, nativeWindowSettings);
        chessGame.Run();
    }
}