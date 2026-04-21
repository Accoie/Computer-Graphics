using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Task1
{
    public class CanabolaApplication : GameWindow
    {
        private Shader _shader;
        private Canabola _canabola;

        public CanabolaApplication(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings)
        {
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            string vertexSource = File.ReadAllText("../../../Shaders/canabola.vert");
            string fragmentSource = File.ReadAllText("../../../Shaders/canabola.frag");
            _shader = new Shader(vertexSource, fragmentSource);

            float step = (float)(Math.PI / 1000);
            _canabola = new Canabola(0, (float)(2 * Math.PI), step);

            GL.ClearColor(1.0f, 1.0f, 1.0f, 1.0f);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            _shader.Use();

            float aspectRatio = (float)Size.X / (float)Size.Y;
            float scale = 2.3f;

            Matrix4 projection = Matrix4.CreateOrthographicOffCenter(
                -aspectRatio * scale,
                aspectRatio * scale,
                -scale,
                scale,
                -1.0f,
                1.0f
            );

            int projectionLoc = _shader.GetUniformLocation("projection");
            GL.UniformMatrix4(projectionLoc, true, ref projection);

            int yOffsetLoc = _shader.GetUniformLocation("yOffset");
            GL.Uniform1(yOffsetLoc, -1.4f);

            _canabola.Draw();
            SwapBuffers();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, Size.X, Size.Y);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            if (KeyboardState.IsKeyPressed(Keys.Escape))
            {
                Close();
            }
        }

        protected override void OnUnload()
        {
            _canabola.Dispose();
            _shader.Dispose();
            base.OnUnload();
        }
    }
}
