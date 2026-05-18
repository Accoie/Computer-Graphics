using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Task3.Shaders;
using Task3.Shapes;

namespace Task3;

public class MainWindow : GameWindow
{
    private Shader _shader = null!;

    private readonly Matrix4 _model = Matrix4.Identity;
    private Matrix4 _view = Matrix4.Identity;
    private Matrix4 _projection = Matrix4.Identity;

    private Vector3 _cameraPos;
    private readonly Vector3 _lightPos = new(3f, 3f, 0f);

    private readonly Vector3 _lightColor = new(1f, 1f, 1f);

    private readonly Pyramid _pyramid = new();

    private bool _isMousePressed;
    private Vector2 _lastMousePos;
    private readonly float _sensitivity = 0.2f;

    private float _verticalAngle;
    private float _horizontalAngle;
    private float _cameraDistance = 5f;

    private readonly float _ambientStrength = 0.3f;
    private readonly float _specularStrength = 0.5f;
    private readonly float _shininess = 32;

    public MainWindow(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) 
        : base(gameWindowSettings, nativeWindowSettings)
    {
    }

    protected override void OnLoad()
    {
        base.OnLoad();

        GL.ClearColor(Color4.Black);
        GL.Enable(EnableCap.DepthTest);

        _shader = new();

        CalculateViewMatrix();
        CalculateProjectionMatrix();

        _pyramid.CreateBuffers();
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);

        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        _shader.Use();
        SetMatrixToShader();
        SetLightToShader();

        PaintParaboloids();

        SwapBuffers();
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);
        GL.Viewport(0, 0, Size.X, Size.Y);
        CalculateProjectionMatrix();
    }

    private void SetMatrixToShader()
    {
        _shader.SetMatrix4("model", _model);
        _shader.SetMatrix4("view", _view);
        _shader.SetMatrix4("projection", _projection);
    }

    private void SetLightToShader()
    {
        _shader.SetVector3("lightPos", _lightPos);
        _shader.SetVector3("lightColor", _lightColor);
        _shader.SetFloat("ambientStrength", _ambientStrength);
        _shader.SetFloat("specularStrength", _specularStrength);
        _shader.SetFloat("shininess", _shininess);
        _shader.SetVector3("viewPos", _cameraPos);
    }

    private void PaintParaboloids()
    {
        IReadOnlyList<Torus> paraboloids = _pyramid.Toruses;
        _shader.SetInt("torusCount", paraboloids.Count);

        for (int i = 0; i < paraboloids.Count; i++)
        {
            _shader.SetVector3($"torusPositions[{i}]", paraboloids[i].Data.Position);
            _shader.SetFloat($"torusMajorRadii[{i}]", paraboloids[i].Data.MajorRadius);
            _shader.SetFloat($"torusMinorRadii[{i}]", paraboloids[i].Data.MinorRadius);
        }

        _pyramid.Paint(_shader);
    }

    private void CalculateViewMatrix()
    {
        UpdateCameraPosition();
    }

    private void CalculateProjectionMatrix()
    {
        _projection = Matrix4.CreatePerspectiveFieldOfView(
            MathHelper.DegreesToRadians(45f),
            Size.X / (float)Size.Y,
            0.1f,
            100f);
    }

    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
        base.OnMouseDown(e);

        if (!_isMousePressed)
        {
            _isMousePressed = true;
            _lastMousePos = new Vector2(MouseState.X, MouseState.Y);
        }
    }

    protected override void OnMouseMove(MouseMoveEventArgs e)
    {
        base.OnMouseMove(e);

        if (!_isMousePressed)
        {
            return;
        }

        float deltaX = e.X - _lastMousePos.X;
        float deltaY = e.Y - _lastMousePos.Y;
        _lastMousePos = new Vector2(e.X, e.Y);

        _horizontalAngle += deltaX * _sensitivity;
        _verticalAngle += deltaY * _sensitivity;

        _verticalAngle = Math.Clamp(_verticalAngle, -89f, 89f);

        UpdateCameraPosition();
    }

    protected override void OnMouseUp(MouseButtonEventArgs e)
    {
        base.OnMouseUp(e);

        if (_isMousePressed)
        {
            _isMousePressed = false;
        }
    }

    protected override void OnKeyDown(KeyboardKeyEventArgs e)
    {
        base.OnKeyDown(e);

        switch (e.Key)
        {
            case Keys.Escape:
                Close();
                break;
            case Keys.W:
                _cameraDistance -= 0.1f;
                UpdateCameraPosition();
                break;
            case Keys.S:
                _cameraDistance += 0.1f;
                UpdateCameraPosition();
                break;
            case Keys.A:
                _horizontalAngle -= 2f;
                UpdateCameraPosition();
                break;
            case Keys.D:
                _horizontalAngle += 2f;
                UpdateCameraPosition();
                break;
        }
    }

    private void UpdateCameraPosition()
    {
        float horizontalAngleRad = MathHelper.DegreesToRadians(_horizontalAngle);
        float verticalAngleRad = MathHelper.DegreesToRadians(_verticalAngle);

        float x = _cameraDistance * (float)(Math.Cos(verticalAngleRad) * Math.Cos(horizontalAngleRad));
        float y = _cameraDistance * (float)(Math.Sin(verticalAngleRad));
        float z = _cameraDistance * (float)(Math.Cos(verticalAngleRad) * Math.Sin(horizontalAngleRad));

        _cameraPos = new Vector3(x, y, z);

        _view = Matrix4.LookAt(_cameraPos, Vector3.Zero, Vector3.UnitY);
    }
}