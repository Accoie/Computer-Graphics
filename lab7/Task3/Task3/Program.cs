using OpenTK.Windowing.Desktop;

namespace Task3
{
    public static class Program
    {
        public static void Main()
        {
            NativeWindowSettings? nativeSettings = NativeWindowSettings.Default;
            nativeSettings.Title = "Morphing";
            nativeSettings.ClientSize = new OpenTK.Mathematics.Vector2i(800, 600);
            
            using (MorphingApp game = new MorphingApp(GameWindowSettings.Default, nativeSettings))
            {
                game.Run();
            }
        }
    }
}