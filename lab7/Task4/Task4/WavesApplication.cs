using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Task4.Services;
using Task4.Shaders;

namespace Task4;

public class WavesApplication : GameWindow
{
    private const string FromImg = "Images/from2.jpg";
    private const string ToImg = "Images/to2.jpg";
    private const float WaveSpeed = 0.5f;
    
    private readonly Shader _shader;

    private readonly Matrix4 _model = Matrix4.Identity;
    private readonly Vector3 _cameraPos = new(0, 0f, 3f);
    private Matrix4 _view = Matrix4.Identity;
    private Matrix4 _projection = Matrix4.Identity;


    private int _canvasVao;
    private int _canvasVertexCount;

    private int _textureFrom;
    private int _textureTo;

    private float _time;
    private Vector2 _clickPosition = new(0.5f, 0.5f);

    public WavesApplication(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) 
        : base(gameWindowSettings, nativeWindowSettings)
    {
        _shader = new();
        UpdateFrequency = 60.0f;
        VSync = VSyncMode.On;
        Load += OnLoad;
        RenderFrame += OnRenderFrame;
        Resize += OnResize;
        MouseDown += OnMouseDown;
    }

    protected override void OnLoad()
    {
        GL.ClearColor(0.0f, 0.0f, 0.0f, 1f);
        GL.Enable(EnableCap.DepthTest);
        
        CalculateViewMatrix();
        CalculateProjectionMatrix();

        CreateCanvas();

        try 
        {
            _textureFrom = TextureLoader.Load(FromImg);

            _textureTo = TextureLoader.Load(ToImg);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading textures: {ex.Message}");
        }
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        _shader.Use();
        
        _shader.SetMatrix4("model", _model);
        _shader.SetMatrix4("view", _view);
        _shader.SetMatrix4("projection", _projection);
        _shader.SetFloat("Time", _time);
        _shader.SetVector2("ClickPos", _clickPosition);

        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2D, _textureFrom);
        
        GL.ActiveTexture(TextureUnit.Texture1);
        GL.BindTexture(TextureTarget.Texture2D, _textureTo);

        _shader.SetInt("textureFrom", 0);
        _shader.SetInt("textureTo", 1);

        PaintCanvas();
        
        SwapBuffers();
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        GL.Viewport(0, 0, Size.X, Size.Y);
        CalculateProjectionMatrix();
    }

    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
        if (e.Button != MouseButton.Left)
        {
            return;
        }

        Vector2 mousePos = new Vector2(MouseState.X, MouseState.Y);

        float uvX = mousePos.X / Size.X;
        float uvY = 1.0f - mousePos.Y / Size.Y;

        _clickPosition = new Vector2(uvX, uvY);
        _time = 0f;
    }
    private void CreateCanvas()
    {
        float[] points =
        [
            -1f,  1f, 0f, 0f, 1f,
            1f,  1f, 0f, 1f, 1f,
            1f, -1f, 0f, 1f, 0f,
            -1f, -1f, 0f, 0f, 0f
        ];

        _canvasVertexCount = 4;
        _canvasVao = GL.GenVertexArray();
        
        GL.BindVertexArray(_canvasVao);

        int vbo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);

        GL.BufferData(BufferTarget.ArrayBuffer, points.Length * sizeof(float), points, BufferUsageHint.StaticDraw);

        ConfigurateShaders();

        GL.BindVertexArray(0);
    }

    private void PaintCanvas()
    {
        GL.BindVertexArray(_canvasVao);
        
        GL.DrawArrays(PrimitiveType.TriangleFan, 0, _canvasVertexCount);
        
        GL.BindVertexArray(0);
    }

    private void ConfigurateShaders()
    {
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
        GL.EnableVertexAttribArray(1);
    }

    private void CalculateViewMatrix()
    {
        _view = Matrix4.LookAt(_cameraPos, Vector3.Zero, Vector3.UnitY);
    }

    private void CalculateProjectionMatrix()
    {
        float aspectRatio = (float)Size.X / Size.Y;
        _projection = Matrix4.CreateOrthographicOffCenter(
            -aspectRatio,
            aspectRatio,
            -1, 1,
            0.1f,
            100f);
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        base.OnUpdateFrame(e);
        _time += (float)e.Time * WaveSpeed;
    }
}