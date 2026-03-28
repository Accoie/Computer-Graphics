using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Task3.Handlers;
using Task3.Strategies;
using Task3.Models;

namespace Task3;

public class LabyrinthGame : GameWindow
{
    private readonly int _mapSize;
    private readonly Player _player;
    private readonly InputHandler _inputHandler;

    private readonly IRenderStrategy[] _renderStrategies;

    public LabyrinthGame(int width, int height, string title)
        : base(GameWindowSettings.Default, new NativeWindowSettings
        {
            ClientSize = new Vector2i(width, height),
            Title = title,
            Profile = ContextProfile.Compatability
        })
    {
        _mapSize = GameConfig.World.LabyrinthMap.GetLength(0);
        _player = CreatePlayer();
        _inputHandler = new InputHandler();
        _renderStrategies = CreateRenderStrategies(GameConfig.World.WallHeight);
    }

    protected override void OnLoad()
    {
        base.OnLoad();
        InitializeGraphics();
        ConfigureLighting();
        CursorState = CursorState.Grabbed;
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);
        GL.Viewport(0, 0, Size.X, Size.Y);

        GL.MatrixMode(MatrixMode.Projection);
        GL.LoadIdentity();

        float aspect = Size.X / (float)Size.Y;
        Matrix4 perspective = Matrix4.CreatePerspectiveFieldOfView(
            MathHelper.DegreesToRadians(GameConfig.Camera.FovDegrees), aspect,
            GameConfig.Camera.NearPlane,
            GameConfig.Camera.FarPlane);
        GL.LoadMatrix(ref perspective);

        GL.MatrixMode(MatrixMode.Modelview);
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);

        if (KeyboardState.IsKeyDown(Keys.Escape))
        {
            Close();
        }

        _player.UpdateRotation(MouseState.X, MouseState.Y);
        
        Vector2 inputDirection = _inputHandler.GetMovementDirection(KeyboardState);
        _player.UpdateMovement(inputDirection, (float)args.Time);
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        GL.Light(LightName.Light0, LightParameter.Position, new Vector4(_player.Position,1.0f));
        SetupCamera();
        RenderScene();
        SwapBuffers();
    }

    private void RenderScene()
    {
        foreach (IRenderStrategy strategy in _renderStrategies)
        {
            strategy.Render(GameConfig.World.LabyrinthMap, _mapSize);
        }
    }

    private void SetupCamera()
    {
        GL.MatrixMode(MatrixMode.Modelview);
        GL.LoadIdentity();
        GL.Rotate(_player.RotationX, 1, 0, 0);
        GL.Rotate(_player.RotationY, 0, 1, 0);
        GL.Translate(-_player.Position.X, -_player.Position.Y, -_player.Position.Z);
    }
    
    private static void InitializeGraphics()
    {
        GL.ClearColor(0.15f, 0.25f, 0.4f, 1f);
        
        GL.Enable(EnableCap.DepthTest);
        GL.Enable(EnableCap.Lighting);
        GL.Enable(EnableCap.Light0);
        GL.Enable(EnableCap.ColorMaterial);
        GL.Enable(EnableCap.Normalize);
        
        GL.Disable(EnableCap.AutoNormal);
    }

    private static void ConfigureLighting()
    {
        GL.Light(LightName.Light0, LightParameter.Ambient, GameConfig.Lighting.LightAmbient);
        GL.Light(LightName.Light0, LightParameter.Diffuse, GameConfig.Lighting.LightDiffuse);
        GL.Light(LightName.Light0, LightParameter.Specular, GameConfig.Lighting.LightSpecular);
        GL.Light(LightName.Light0, LightParameter.ConstantAttenuation, GameConfig.Lighting.ConstantAttenuation);
        GL.Light(LightName.Light0, LightParameter.LinearAttenuation, GameConfig.Lighting.LinearAttenuation);
        GL.Light(LightName.Light0, LightParameter.QuadraticAttenuation, GameConfig.Lighting.QuadraticAttenuation);
    }
    
    private static Player CreatePlayer()
    {
        return new Player(
            startPosition: GameConfig.Player.StartPosition,
            startRotation: GameConfig.Player.StartRotation,
            moveSpeed: GameConfig.Player.MoveSpeed,
            rotationSensitivity: GameConfig.Player.MouseSensitivity,
            radius: GameConfig.Player.Radius
        );
    }
    
    private static IRenderStrategy[] CreateRenderStrategies(float wallHeight)
    {
        return [new FloorRenderStrategy(), 
            new CeilingRenderStrategy(wallHeight), 
            new WallRenderStrategy(wallHeight)];
    }
}