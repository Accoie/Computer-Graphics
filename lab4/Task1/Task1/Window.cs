using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Task1
{
    public class Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
        : GameWindow(gameWindowSettings, nativeWindowSettings)
    {
        private bool _leftButtonPressed;
        private float _mouseX;
        private float _mouseY;

        private readonly Icosahedron _shape = new();

        protected override void OnLoad()
        {
            GL.ClearColor(0f, 0f,0f, 1f);

            GL.Enable(EnableCap.DepthTest);

            GL.Light(LightName.Light0, LightParameter.Position, new Vector4(10f, 0f, 10f, 0f));
            GL.Light(LightName.Light0, LightParameter.Diffuse, new Vector4(1f, 1f, 1f, 1f));

            GL.Enable(EnableCap.Lighting);
            GL.Enable(EnableCap.Light0);

            GL.Enable(EnableCap.ColorMaterial);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            GL.Translate(0f,0f,-3f);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (e.Button == MouseButton.Left)
            {
                _leftButtonPressed = true;

                _mouseX = MousePosition.X;
                _mouseY = MousePosition.Y;
            }
        }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            if (!_leftButtonPressed)
            {
                return;
            }

            float dx = e.X - _mouseX;
            float dy = e.Y - _mouseY;

            float rotateX = dy * 180 / Size.X;
            float rotateY = dx * 180 / Size.Y;
            RotateCamera(rotateX, rotateY);

            _mouseX = e.X;
            _mouseY = e.Y;
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            _leftButtonPressed = false;
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            int width = e.Width;
            int height = e.Height;

            GL.Viewport(0, 0, width, height);

            SetupProjectionMatrix(width, height);

            GL.MatrixMode(MatrixMode.Modelview);
            OnRenderFrame(new FrameEventArgs());
        }
        
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GL.MatrixMode(MatrixMode.Modelview);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.LineWidth(1);
            
            _shape.Draw();
            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            OnRefresh();
        }
        
        private void SetupProjectionMatrix(int width, int height)
        {
            const double frustumWidth = 1;
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
    
            double aspectRatio = width / (double)height;

            bool isPortraitOrientation = aspectRatio < 1 && aspectRatio != 0;
            
            double frustumHeight = isPortraitOrientation? frustumWidth / aspectRatio : aspectRatio;
    
            GL.Frustum(
                -frustumWidth, frustumWidth,
                -frustumHeight, frustumHeight,
                zNear: 1, zFar: 5
            );
        }
        
        private void RotateCamera(float x, float y)
        { // разобраться что-то такое modelview
            GL.MatrixMode(MatrixMode.Modelview);

            GL.GetFloat(GetPName.ModelviewMatrix, out Matrix4 modelView);

            Vector3 xAxis = new Vector3(modelView[0, 0], modelView[1,0], modelView[2,0]);
            Vector3 yAxis = new Vector3(modelView[0, 1], modelView[1, 1], modelView[2, 1]);

            GL.Rotate(x, xAxis);
            GL.Rotate(y, yAxis);
        }
    }
}