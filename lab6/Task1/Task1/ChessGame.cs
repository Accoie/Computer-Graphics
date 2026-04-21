using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using Task1.Models.Data;
using Task1.Services;
using Task1.Shaders;

namespace Task1;

public class ChessGame(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
    : GameWindow(gameWindowSettings, nativeWindowSettings)
{
    private Shader _shader = null!;
    private readonly Matrix4 _model = Matrix4.Identity;
    private Matrix4 _view = Matrix4.Identity;
    private Matrix4 _projection = Matrix4.Identity;
    
    private bool _isDragging;
    private Vector2 _lastMousePos;
    private Vector3 _cameraPos = new(0, 20f, 15f);
    
    private readonly Vector3 _lightPos = new(5.0f, 0.0f, 5.0f);
    private readonly Vector3 _lightColor = new(1.0f, 1.0f, 1.0f);
    private readonly float _ambientStrength = 0.3f;

    private ChessPainter _chessPainter = null!;

    protected override void OnLoad()
    {
        base.OnLoad();
        
        GL.ClearColor(0.2f, 0.2f, 0.2f, 1.0f);
        GL.Enable(EnableCap.DepthTest);
        
        int vertexBufferObj = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObj);
        int vertexArrayObj = GL.GenVertexArray();
        GL.BindVertexArray(vertexArrayObj);
        
        _shader = new Shader();
        _chessPainter = new ChessPainter(GetChessMoves());
        UpdateViewMatrix();         
        CalculateProjectionMatrix();
    }

    protected override void OnUnload()
    {
        _chessPainter.Dispose();
        base.OnUnload();
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);
        GL.Viewport(0, 0, Size.X, Size.Y);
        CalculateProjectionMatrix();
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);
        
        _chessPainter.Update((float)args.Time);
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);
        
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        
        _shader.Use();
        _shader.SetMatrix4("model", _model);
        _shader.SetMatrix4("view", _view);
        _shader.SetMatrix4("projection", _projection);
        _shader.SetVector3("lightPos", _lightPos);
        _shader.SetVector3("lightColor", _lightColor);
        _shader.SetFloat("ambientStrength", _ambientStrength);
        
        _chessPainter.Paint(_shader);
        
        SwapBuffers();
    }

    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
        base.OnMouseDown(e);
    
        if (_isDragging)
        {
            return;
        }

        _lastMousePos = new Vector2(MouseState.X, MouseState.Y);
        _isDragging = true;
    }


    protected override void OnMouseUp(MouseButtonEventArgs e)
    {
        base.OnMouseUp(e);
        
        if (!_isDragging)
        {
            return;
        }

        _isDragging = false;
    }

    protected override void OnMouseMove(MouseMoveEventArgs e)
    {
        base.OnMouseMove(e);
        
        if (!_isDragging)
        {
            return;
        }

        float dx = (_lastMousePos.X - e.X) / 20f;
        _lastMousePos = new Vector2(e.X, e.Y);
        
        _cameraPos = RotateVector(_cameraPos, dx);
        UpdateViewMatrix();
    }

    private void UpdateViewMatrix()
    {
        _view = Matrix4.LookAt(_cameraPos, Vector3.Zero, Vector3.UnitZ);
    }

    private static Vector3 RotateVector(Vector3 vector, float angle)
    {
        float x = vector.X;
        float y = vector.Y;
        
        float sin = (float)Math.Sin(angle);
        float cos = (float)Math.Cos(angle);
        
        return new Vector3(x * cos - y * sin, x * sin + y * cos, vector.Z);
    }

    private void CalculateProjectionMatrix()
    {
        float aspectRatio = Size.X / (float)Size.Y;
        _projection = Matrix4.CreatePerspectiveFieldOfView(
            MathHelper.DegreesToRadians(45f),
            aspectRatio, 
            0.1f, 
            100f);
    }
    
    private static ChessMove[] GetChessMoves()
    {
        return
        [
            // 1. e4 e5
            new ChessMove(4, 1, 4, 3), 
            new ChessMove(4, 6, 4, 4),  

            // 2. Bc4 Nc6
            new ChessMove(5, 0, 2, 3),  
            new ChessMove(1, 7, 2, 5), 

            // 3. Qh5 Nf6
            new ChessMove(3, 0, 7, 4), 
            new ChessMove(6, 7, 5, 5), 

            // 4. Qxf7#
            new ChessMove(7, 4, 5, 6)
        ];
    }
}