using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace Task4;

public class Program
{
    public static void Main()
    {
        NativeWindowSettings nativeWindowSettings = new NativeWindowSettings
        {
            ClientSize = new Vector2i(982, 853),
            Title = "Waves",
            Flags = ContextFlags.Default,
            API = ContextAPI.OpenGL,
            APIVersion = new Version(3, 3),
            Profile = ContextProfile.Core,
            IsEventDriven = false
        };

        WavesApplication gameWindow = new WavesApplication(GameWindowSettings.Default, nativeWindowSettings);
        
        gameWindow.Run();
    }
}