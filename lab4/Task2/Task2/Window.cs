using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Task2;

public class Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
    : GameWindow(gameWindowSettings, nativeWindowSettings)
{
    private bool _leftButtonPressed;
    private float _mouseX;
    private float _mouseY;

    private readonly MobiusStrip _shape = new();

    protected override void OnLoad()
    {
        base.OnLoad();

        GL.ClearColor(0f, 0f, 0f, 1f);

        GL.Enable(EnableCap.DepthTest);

        GL.Light(LightName.Light1, LightParameter.Position, new Vector4(0f, 20f, 20f, 0f));
        GL.Light(LightName.Light1, LightParameter.Diffuse, new Vector4(0.8f, 0.8f, 0.8f, 1f));
        GL.Light(LightName.Light1, LightParameter.Ambient, new Vector4(0.3f, 0.3f, 0.3f, 1f));

        GL.Enable((EnableCap.AutoNormal));
        GL.Enable(EnableCap.Lighting);
        GL.Enable(EnableCap.Light1);

        GL.Enable(EnableCap.ColorMaterial);

        GL.LoadIdentity();
        Matrix4 matrix = Matrix4.LookAt(
            0f, 0f, 2.5f,
            0f, 0f, 0f,
            0, 1, 0);
        GL.LoadMatrix(ref matrix);
    }

    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
        if (e.Button == MouseButton.Left)
        {
            _leftButtonPressed = true;

            _mouseX = MousePosition.X;
            _mouseY = MousePosition.Y;
        }

        base.OnMouseDown(e);
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

    private void RotateCamera(float x, float y)
    {
        GL.MatrixMode(MatrixMode.Modelview);

        GL.GetFloat(GetPName.ModelviewMatrix, out Matrix4 modelView);

        Vector3 xAxis = new Vector3(modelView[0, 0], modelView[1, 0], modelView[2, 0]);
        Vector3 yAxis = new Vector3(modelView[0, 1], modelView[1, 1], modelView[2, 1]);

        GL.Rotate(x, xAxis);
        GL.Rotate(y, yAxis);
    }

    protected override void OnMouseUp(MouseButtonEventArgs e)
    {
        _leftButtonPressed = false;
        base.OnMouseUp(e);
    }

    protected override void OnMouseLeave()
    {
        _leftButtonPressed = false;
        base.OnMouseLeave();
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        int width = e.Width;
        int height = e.Height;

        GL.Viewport(0, 0, width, height);

        SetupProjectionMatrix(width, height);

        GL.MatrixMode(MatrixMode.Modelview);
        base.OnResize(e);

        OnRenderFrame(new FrameEventArgs());
    }

    private void SetupProjectionMatrix(int width, int height)
    {
        GL.MatrixMode(MatrixMode.Projection);
        GL.LoadIdentity();
    
        double aspectRatio = width / (double)height;

        bool isPortraitOrientation = aspectRatio < 1 && aspectRatio != 0;

        double frustumWidth = 1;
        double frustumHeight;
        if (isPortraitOrientation)
        {
            frustumHeight = frustumWidth / aspectRatio;
        }
        else
        {
            frustumHeight = frustumWidth;
            frustumWidth = frustumHeight * aspectRatio;
        }
        
        GL.Frustum(
            -frustumWidth, frustumWidth,
            -frustumHeight, frustumHeight,
            zNear: 1, zFar: 5
        );
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        GL.MatrixMode(MatrixMode.Modelview);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        _shape.Draw();

        SwapBuffers();
        base.OnRenderFrame(args);
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);

        OnRefresh();
    }
}