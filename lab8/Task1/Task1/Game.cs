using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Task1.Configs;
using Task1.Shaders;

namespace Task1;

public class Game() : GameWindow(GameWindowSettings.Default, new NativeWindowSettings
{
    ClientSize = new Vector2i(800, 600),
    Title = "Phong",
    APIVersion = new Version(3, 3),
    Profile = ContextProfile.Core,
    Flags = ContextFlags.ForwardCompatible
})
{
    private Shader _shader = null!;
    private int _vao, _vbo, _ebo;

    private float _rotationX;
    private float _rotationY;
    private float _scale = 1.0f;

    private readonly Material _material = new();
    private readonly Light _light = new();
    private readonly Vector3 _cameraPos = new(0.0f, 0.0f, 5.0f);

    private Vector2 _lastMousePos;
    private bool _isDragging;

    protected override void OnLoad()
    {
        base.OnLoad();

        Console.WriteLine($"OpenGL Version: {GL.GetString(StringName.Version)}");
        Console.WriteLine($"Shader Version: {GL.GetString(StringName.ShadingLanguageVersion)}");

        GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
        GL.Enable(EnableCap.DepthTest);
        GL.Disable(EnableCap.CullFace);
        GL.PolygonMode(TriangleFace.FrontAndBack, PolygonMode.Fill);
        try
        {
            _shader = new Shader();
            Console.WriteLine("Shaders compiled and linked successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Shader Error: {ex.Message}");
            throw;
        }

        _vao = GL.GenVertexArray();
        _vbo = GL.GenBuffer();
        _ebo = GL.GenBuffer();

        GL.BindVertexArray(_vao);
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, Config.Vertices.Length * sizeof(float), Config.Vertices, BufferUsageHint.StaticDraw);
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, Config.Indices.Length * sizeof(uint), Config.Indices, BufferUsageHint.StaticDraw);

        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
        GL.EnableVertexAttribArray(1);

        GL.BindVertexArray(0);

        GL.Viewport(0, 0, Size.X, Size.Y);
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);

        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        _shader.Use();

        GL.PolygonMode(TriangleFace.FrontAndBack, PolygonMode.Fill);
        Matrix4 model = Matrix4.Identity;
        model *= Matrix4.CreateScale(_scale);
        model *= Matrix4.CreateRotationX(MathHelper.DegreesToRadians(_rotationX));
        model *= Matrix4.CreateRotationY(MathHelper.DegreesToRadians(_rotationY));

        Matrix4 view = Matrix4.LookAt(_cameraPos, Vector3.Zero, Vector3.UnitY);
        Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(
            MathHelper.DegreesToRadians(45.0f), Size.X / (float)Size.Y, 0.1f, 100.0f);

        _shader.SetMatrix4("model", model);
        _shader.SetMatrix4("view", view);
        _shader.SetMatrix4("projection", projection);

        _shader.SetVector3("materialAmbient", _material.Ambient);
        _shader.SetVector3("materialDiffuse", _material.Diffuse);
        _shader.SetVector3("materialSpecular", _material.Specular);
        _shader.SetFloat("materialShininess", _material.Shininess);

        _shader.SetVector3("lightPos", _light.Position);
        _shader.SetVector3("lightAmbient", _light.Ambient);
        _shader.SetVector3("lightDiffuse", _light.Diffuse);
        _shader.SetVector3("lightSpecular", _light.Specular);
        _shader.SetVector3("viewPos", _cameraPos);

        GL.BindVertexArray(_vao);
        GL.DrawElements(PrimitiveType.Triangles, 36, DrawElementsType.UnsignedInt, 0);
        GL.BindVertexArray(0);

        GL.PolygonMode(TriangleFace.FrontAndBack, PolygonMode.Fill);
        SwapBuffers();
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);

        KeyboardState? input = KeyboardState;
        MouseState? mouse = MouseState;

        if (input.IsKeyDown(Keys.Escape))
            Close();

        if (input.IsKeyDown(Keys.Up))    _light.Position += new Vector3(0, 0.1f, 0);
        if (input.IsKeyDown(Keys.Down))  _light.Position -= new Vector3(0, 0.1f, 0);
        if (input.IsKeyDown(Keys.Left))  _light.Position -= new Vector3(0.1f, 0, 0);
        if (input.IsKeyDown(Keys.Right)) _light.Position += new Vector3(0.1f, 0, 0);

        if (mouse.IsButtonDown(MouseButton.Left))
        {
            if (!_isDragging)
            {
                _isDragging = true;
                _lastMousePos = mouse.Position;
            }
            else
            {
                float dx = mouse.Position.X - _lastMousePos.X;
                float dy = mouse.Position.Y - _lastMousePos.Y;
                _rotationY += dx * 0.5f;
                _rotationX += dy * 0.5f;
                _lastMousePos = mouse.Position;
            }
        }
        else
        {
            _isDragging = false;
        }
    }

    protected override void OnMouseWheel(MouseWheelEventArgs e)
    {
        _scale = Math.Clamp(_scale + e.OffsetY * 0.1f, 0.1f, 10.0f);
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);
        GL.Viewport(0, 0, e.Width, e.Height);
    }

    protected override void OnUnload()
    {
        base.OnUnload();
        if (_vbo != 0) GL.DeleteBuffer(_vbo);
        if (_ebo != 0) GL.DeleteBuffer(_ebo);
        if (_vao != 0) GL.DeleteVertexArray(_vao);
    }
}