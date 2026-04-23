using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL;
using Task2.Figures;
using Task2.Shaders;

namespace Task2;

public class UssrFlagWindow : GameWindow
{
    private Shader _shader;
    
    private Matrix4 _model = Matrix4.Identity;
    private Matrix4 _view = Matrix4.Identity;
    private Matrix4 _projection = Matrix4.Identity;

    private Vector3 _cameraPos = new(0, 0f, 3f);

    private const float InternalRadius = 0.2f;
    private const float OuterRadius = 0.25f;
    private Vector3 _backgroundColor = new(1);
    private Vector3 _circleColor = new(0);
    private Vector2 _center = new(0, 0);

    private List<Figure> _figures;

    public UssrFlagWindow(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings, List<Figure> figures)
        : base(gameWindowSettings, nativeWindowSettings)
    {
        _figures = figures;
    }

    protected override void OnLoad()
    {
        base.OnLoad();

        GL.ClearColor(0.6f, 0f, 0f, 1f);
        GL.Enable(EnableCap.DepthTest);

        _shader = new Shader();

        CalculateViewMatrix();
        CalculateProjectionMatrix();

        _figures.ForEach(f => f.CreatePoints());
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);

        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        _shader.Use();
        _shader.SetMatrix4("model", _model);
        _shader.SetMatrix4("view", _view);
        _shader.SetMatrix4("projection", _projection);
        _shader.SetFloat("OuterRadius", OuterRadius);
        _shader.SetFloat("InternalRadius", InternalRadius);
        _shader.SetVector2("Center", _center);
        _shader.SetVector3("BackroundColor", _backgroundColor);
        _shader.SetVector3("CircleColor", _circleColor);

        _figures.ForEach(f => f.Paint());

        SwapBuffers();
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);

        GL.Viewport(0, 0, Size.X, Size.Y);
        CalculateProjectionMatrix();
    }

    protected override void OnUnload()
    {
        _shader.Dispose();
        base.OnUnload();
    }

    private void CalculateViewMatrix()
    {
        _view = Matrix4.LookAt(_cameraPos, Vector3.UnitZ, Vector3.UnitY);
    }

    private void CalculateProjectionMatrix()
    {
        float aspectRatio = (float)ClientSize.X / ClientSize.Y;
        _projection = Matrix4.CreateOrthographicOffCenter(
            -aspectRatio,
            aspectRatio,
            -1, 1,
            0.1f,
            100f);
    }
}
