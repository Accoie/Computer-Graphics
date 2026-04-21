using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace Task1
{
    public class Program
    {
        public static void Main()
        {
            NativeWindowSettings nativeWindowSettings = new NativeWindowSettings()
            {
                ClientSize = new OpenTK.Mathematics.Vector2i(1920, 1080),
                Title = "Canabola - Vertex Shader Transformation",
                APIVersion = new Version(3, 3),
                Flags = ContextFlags.ForwardCompatible,
                Profile = ContextProfile.Core
            };

            using (CanabolaApplication app = new CanabolaApplication(GameWindowSettings.Default, nativeWindowSettings))
            {
                app.Run();
            }
        }
    }
}