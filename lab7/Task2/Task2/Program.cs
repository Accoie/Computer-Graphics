using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using Task2.Figures;

namespace Task2;

class Program
{
    static void Main()
    {
        List<Figure> figures = new List<Figure>
        {
            new Hammer(),
            new Sickle(),
            new Star()
        };

        NativeWindowSettings nativeWindowSettings = new NativeWindowSettings()
        {
            ClientSize = new Vector2i(1920, 1080),
            Title = "USSR!",
            Flags = ContextFlags.Default,
            APIVersion = new Version(3, 3),
            Profile = ContextProfile.Core
        };

        using UssrFlagWindow window = new UssrFlagWindow(GameWindowSettings.Default, nativeWindowSettings, figures);
        window.Run();
    }
}