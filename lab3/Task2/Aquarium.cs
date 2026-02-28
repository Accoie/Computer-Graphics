using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using Task2.Components;
using Task2.Strategies.FishDrawingStrategies;
using Task2.Strategies.StoneDrawingStrategies;

namespace Task2;

public class AquariumWindow : GameWindow
{
    private const float WindowWidth = 800f;
    private const float WindowHeight = 600f;

    private static readonly Vector2[] FishBasePositions =
    [
        new Vector2(370.0f, -100.0f),
        new Vector2(250.0f, 300.0f),
        new Vector2(-400.0f, -200.0f),
        new Vector2(-170.0f, 50.0f),
        new Vector2(440.0f, -301.0f)
    ];

    private readonly float[] _fishSpeeds = [2.5f, 2.2f, 2.8f, 2.0f, 3.0f];
    private float[] _fishOffsets = [-1450, -850, -1200, -950, -600];
    private float[] _bubbleTimers = new float[ 5 ];

    private readonly List<Bubble> _bubbles = [];
    private readonly Random _random = new Random();

    private readonly List<Fish> _fishes = [];
    private readonly List<Stone> _stones = [];

    public AquariumWindow( GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings )
        : base( gameWindowSettings, nativeWindowSettings ) { }

    protected override void OnLoad()
    {
        base.OnLoad();
        GL.ClearColor( 0.6f, 0.77f, 0.57f, 1.00f );
        GL.Enable( EnableCap.Blend );
        GL.BlendFunc( BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha );

        InitializeFishes();
        InitializeStones();
    }

    protected override void OnResize( ResizeEventArgs e )
    {
        base.OnResize( e );
        GL.Viewport( 0, 0, Size.X, Size.Y );
        AdjustProjection();
        SwapBuffers();
    }

    protected override void OnUpdateFrame( FrameEventArgs args )
    {
        base.OnUpdateFrame( args );
        float deltaTime = ( float )args.Time;

        UpdateFishPositions( deltaTime );
        UpdateBubbles( deltaTime );

        for ( int i = 0; i < _bubbleTimers.Length; i++ )
        {
            _bubbleTimers[ i ] += deltaTime;
        }

        GenerateBubblesFromFish();
    }

    protected override void OnRenderFrame( FrameEventArgs args )
    {
        GL.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );
        DrawAquarium();
        DrawBubbles();
        SwapBuffers();
        base.OnRenderFrame( args );
    }

    protected override void OnUnload()
    {
        base.OnUnload();
        GL.ClearColor( Color4.White );
    }
    
    private void InitializeFishes()
    {
        _fishes.Add(new Fish(FishBasePositions[0], new Vector2(120.0f, 80.0f), _fishOffsets[0], new Fish1DrawingStrategy()));
        _fishes.Add(new Fish(FishBasePositions[1], new Vector2(150.0f, 40.0f), _fishOffsets[1], new Fish2DrawingStrategy()));
        _fishes.Add(new Fish(FishBasePositions[2], new Vector2(100.0f, 60.0f), _fishOffsets[2], new Fish3DrawingStrategy()));
        _fishes.Add(new Fish(FishBasePositions[3], new Vector2(120.0f, 80.0f), _fishOffsets[3], new Fish4DrawingStrategy()));
        _fishes.Add(new Fish(FishBasePositions[4], new Vector2(100.0f, 60.0f), _fishOffsets[4], new Fish5DrawingStrategy()));
    }

    private void InitializeStones()
    {
        _stones.Add(new Stone(new Vector2(370.0f, -600.0f), new Vector2(100.0f, 100.0f), new Stone2DrawingStrategy()));
        _stones.Add(new Stone(new Vector2(-240.0f, -600.0f), new Vector2(50.0f, 50.0f), new Stone4DrawingStrategy()));
        _stones.Add(new Stone(new Vector2(100.0f, -600.0f), new Vector2(60.0f, 30.0f), new Stone2DrawingStrategy()));
        _stones.Add(new Stone(new Vector2(600.0f, -600.0f), new Vector2(40.0f, 20.0f), new Stone2DrawingStrategy()));
        _stones.Add(new Stone(new Vector2(60.0f, -600.0f), new Vector2(60.0f, 40.0f), new Stone1DrawingStrategy()));
        _stones.Add(new Stone(new Vector2(-550.0f, -600.0f), new Vector2(50.0f, 50.0f), new Stone3DrawingStrategy()));
        _stones.Add(new Stone(new Vector2(200.0f, -600.0f), new Vector2(40.0f, 40.0f), new Stone4DrawingStrategy()));
        _stones.Add(new Stone(new Vector2(500.0f, -600.0f), new Vector2(70.0f, 50.0f), new Stone1DrawingStrategy()));
        _stones.Add(new Stone(new Vector2(440.0f, -600.0f), new Vector2(40.0f, 40.0f), new Stone5DrawingStrategy()));
        _stones.Add(new Stone(new Vector2(630.0f, -600.0f), new Vector2(40.0f, 40.0f), new Stone5DrawingStrategy()));
    }
    
    private void GenerateBubblesFromFish()
    {
        float[] bubbleIntervals = [2.9f, 1.7f, 1.7f, 2.2f, 3.9f];

        for ( int i = 0; i < 5; i++ )
        {
            if ( _bubbleTimers[ i ] > bubbleIntervals[ i ] )
            {
                float fishX = FishBasePositions[ i ].X - _fishOffsets[ i ];
                float fishY = FishBasePositions[ i ].Y;
                GenerateFishBubbles(fishX, fishY );
                _bubbleTimers[ i ] = 0f;
            }
        }
    }

    private void GenerateFishBubbles(float fishX, float fishY )
    {
        for ( int i = 0; i < 1; i++ )
        {
            float offsetX = _random.Next( 10, 30 );
            float offsetY = _random.Next( -20, 20 );
            float bubbleX = fishX + offsetX;
            float bubbleY = fishY + offsetY;
            float size = _random.Next( 8, 15 );
            float speedY = _random.Next( 40, 100 );
            float speedX = _random.Next( -5, 5 );

            _bubbles.Add( new Bubble(
                new Vector2( bubbleX, bubbleY ),
                new Vector2( speedX, speedY ),
                new Vector2( 0, 1 ),
                size
            ) );
        }
    }

    private void UpdateFishPositions( float deltaTime )
    {
        float aspectRatio = ( float )Size.X / Size.Y;
        float visibleHalfWidth = WindowWidth * aspectRatio;
        float leftBound = -visibleHalfWidth - 100f;
        float rightResetPos = visibleHalfWidth + 100f;

        for ( int i = 0; i < _fishOffsets.Length; i++ )
        {
            _fishOffsets[ i ] += _fishSpeeds[ i ] * deltaTime * 100;

            if ( FishBasePositions[ i ].X - _fishOffsets[ i ] < leftBound )
            {
                _fishOffsets[ i ] = FishBasePositions[ i ].X - rightResetPos;
            }

            _fishes[i].Offset = _fishOffsets[i];
        }
    }

    private void UpdateBubbles( float deltaTime )
    {
        for ( int i = _bubbles.Count - 1; i >= 0; i-- )
        {
            Bubble bubble = _bubbles[ i ];
            bubble.Position += bubble.Speed * bubble.Direction * deltaTime;
            bubble.Position.X += ( float )Math.Sin( bubble.LifeTime * 3 ) * deltaTime * 3;
            bubble.LifeTime -= deltaTime;

            if ( bubble.Position.Y > WindowHeight ||
                bubble.Position.X > WindowWidth ||
                bubble.Position.X < -WindowWidth ||
                bubble.LifeTime <= 0 )
            {
                _bubbles.RemoveAt( i );
            }
        }
    }

    private void DrawBubbles()
    {
        foreach ( Bubble bubble in _bubbles )
        {
            DrawBubble( bubble.Position.X, bubble.Position.Y, bubble.Size );
        }
    }

    private void DrawBubble( float x, float y, float size )
    {
        GL.PushMatrix();
        GL.Translate( x, y, 0 );

        DrawEllipse( 0, 0, size, size, new Color4( 1.0f, 1.0f, 1.0f, 0.4f ) );
        DrawEllipse( 0, 0, size, size, new Color4( 1.0f, 1.0f, 1.0f, 0.8f ), PrimitiveType.LineLoop );

        GL.Begin( PrimitiveType.Points );
        GL.Color3( 1.0f, 1.0f, 1.0f );
        GL.Vertex2( size * 0.3f, size * 0.3f );
        GL.End();

        GL.PopMatrix();
    }

    private void AdjustProjection()
    {
        float aspectRatio = ( float )Size.X / Size.Y;
        Matrix4 projection = Matrix4.CreateOrthographic( 1600 * aspectRatio, 1200, -1, 1 );
        GL.MatrixMode( MatrixMode.Projection );
        GL.LoadMatrix( ref projection );
    }

    private void DrawEllipse( float x, float y, float radiusX, float radiusY, Color4 color, PrimitiveType primitiveType = PrimitiveType.Polygon )
    {
        GL.Begin( primitiveType );
        GL.Color4( color );

        int segments = 360;

        for ( int i = 0; i < segments; i++ )
        {
            float degInRad = i * ( float )Math.PI / 180;
            GL.Vertex2( x + Math.Cos( degInRad ) * radiusX, y + Math.Sin( degInRad ) * radiusY );
        }
        GL.End();
    }

    private void DrawAquarium()
    {
        DrawPlants();
        DrawStones();
        DrawFish();
    }

    private void DrawPlants()
    {
        DrawPlant( -500.0f, -600.0f, 60.0f, 450.0f );
        DrawPlant( -100.0f, -600.0f, 50.0f, 250.0f );
        DrawPlant( 100.0f, -600.0f, 60.0f, 300.0f );
        DrawPlant( 300.0f, -600.0f, 20.0f, 150.0f );
        DrawPlant( 500.0f, -600.0f, 60.0f, 550.0f );
        DrawPlant( 370.0f, -600.0f, 100.0f, 750.0f );
        DrawPlant( -700.0f, -600.0f, 80.0f, 350.0f );
    }

    private void DrawStones()
    {
        foreach (Stone stone in _stones)
        {
            stone.Draw();
        }
    }

    private void DrawFish()
    {
        foreach (Fish fish in _fishes)
        {
            fish.Draw();
        }
    }

    private void DrawPlant( float x, float y, float width, float height )
    {
        GL.Begin( PrimitiveType.Quads );
        GL.Color3( 0.0f, 0.5f, 0.0f );
        float offset = 10.0f;

        for ( float i = 0; i < width; i += offset )
        {
            GL.Vertex2( x + i, y );
            GL.Vertex2( x + i + offset, y );
            GL.Vertex2( x + i + offset, y + height * ( float )Math.Sin( ( i + offset / 2 ) * Math.PI / width * 3.2 ) );
            GL.Vertex2( x + i, y + height * ( float )Math.Sin( ( i + offset / 2 ) * Math.PI / width * 3 ) );
        }
        GL.End();
    }
}