using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace Task2;

class Program
{
    static void Main()
    {

        var nativeWinSettings = new NativeWindowSettings()
        {
            ClientSize = new Vector2i( 600, 600 ),
            Location = new Vector2i( 30, 30 ),
            WindowBorder = WindowBorder.Resizable,
            WindowState = WindowState.Normal,
            Title = "Аквариум",
            Flags = ContextFlags.Default,
            APIVersion = new Version( 3, 3 ),
            Profile = ContextProfile.Compatability,
            API = ContextAPI.OpenGL,
            NumberOfSamples = 0
        };

        using ( AquariumWindow game = new AquariumWindow( GameWindowSettings.Default, nativeWinSettings ) )
        {
            game.Run();
        }
    }
}