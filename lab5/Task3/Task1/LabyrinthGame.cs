using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Task1;
using Task1.Handlers;
using Task1.Models;
using Task1.Services;
using Task1.Strategies;

public class LabyrinthGame : GameWindow
{
    private readonly InputHandler _inputHandler;
    private readonly int _mapSize;
    private readonly Player _player;

    private readonly IRenderStrategy[] _renderStrategies;
    private readonly int[] _wallTexturesId;

    private int _floorTextureId;
    private int _shadowMapTextureId;
    private int _skyboxTextureId;

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

        _renderStrategies = new IRenderStrategy[3];
        _wallTexturesId = new int[6];
    }

    protected override void OnLoad()
    {
        base.OnLoad();

        LoadTextures();

        _renderStrategies[0] = new SkyboxRenderStrategy(_skyboxTextureId);
        _renderStrategies[1] = new FloorRenderStrategy(_floorTextureId);
        _renderStrategies[2] = new WallRenderStrategy(GameConfig.World.WallHeight, _wallTexturesId, _shadowMapTextureId);

        InitializeGraphics();
        ConfigureLighting();
        
        CursorState = CursorState.Grabbed;
    }


    protected override void OnUnload()
    {
        TextureService.DeleteTexture(_floorTextureId);
        TextureService.DeleteTexture(_skyboxTextureId);

        foreach (int tex in _wallTexturesId)
        {
            TextureService.DeleteTexture(tex);
        }

        TextureService.DeleteTexture(_shadowMapTextureId);

        base.OnUnload();
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);
        GL.Viewport(0, 0, Size.X, Size.Y);

        GL.MatrixMode(MatrixMode.Projection);
        GL.LoadIdentity();

        float aspect = Size.X / (float)Size.Y;
        Matrix4 perspective = Matrix4.CreatePerspectiveFieldOfView(
            MathHelper.DegreesToRadians(GameConfig.Camera.FovDegrees),
            aspect,
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

        GL.Light(LightName.Light0, LightParameter.Position, new Vector4(_player.Position, 1.0f));

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

        GL.Enable(EnableCap.Texture2D);

        GL.ActiveTexture(TextureUnit.Texture1);
        GL.Enable(EnableCap.Texture2D);
        GL.ActiveTexture(TextureUnit.Texture0);

        GL.Disable(EnableCap.AutoNormal);

        if (GameConfig.Fog.Enabled)
        {
            EnableFog();
        }
    }
    
    private static void EnableFog()
    {
        GL.Enable(EnableCap.Fog);
        GL.Fog(FogParameter.FogMode, (int)FogMode.Exp2);
        GL.Fog(FogParameter.FogDensity, GameConfig.Fog.Density);
        GL.Fog(FogParameter.FogColor, GameConfig.Fog.Color);
        GL.Fog(FogParameter.FogStart, GameConfig.Fog.Start);
        GL.Fog(FogParameter.FogEnd, GameConfig.Fog.End);
    }
    
    private void LoadTextures()
    {
        _floorTextureId = TextureService.LoadTexture("textures/floor.jpg");

        _wallTexturesId[0] = TextureService.LoadTexture("textures/wall1.jpg");
        _wallTexturesId[1] = TextureService.LoadTexture("textures/wall2.jpg");
        _wallTexturesId[2] = TextureService.LoadTexture("textures/wall3.jpg");
        _wallTexturesId[3] = TextureService.LoadTexture("textures/wall4.jpg");
        _wallTexturesId[4] = TextureService.LoadTexture("textures/wall5.jpg");
        _wallTexturesId[5] = TextureService.LoadTexture("textures/wall6.jpg");

        _skyboxTextureId = TextureService.LoadTexture("textures/skybox.jpg");

        _shadowMapTextureId = TextureService.LoadShadowMap("textures/shadowmap.jpg");
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
            GameConfig.Player.StartPosition,
            GameConfig.Player.StartRotation,
            GameConfig.Player.MoveSpeed,
            GameConfig.Player.MouseSensitivity,
            GameConfig.Player.Radius
        );
    }
}