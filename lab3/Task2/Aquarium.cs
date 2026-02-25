using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using Task2.Components;

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

    public AquariumWindow( GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings )
        : base( gameWindowSettings, nativeWindowSettings ) { }

    protected override void OnLoad()
    {
        base.OnLoad();
        GL.ClearColor( 0.6f, 0.77f, 0.57f, 1.00f );
        GL.Enable( EnableCap.Blend );
        GL.BlendFunc( BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha );
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

    void GenerateFishBubbles(float fishX, float fishY )
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

    void UpdateFishPositions( float deltaTime )
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
        }
    }

    void UpdateBubbles( float deltaTime )
    {
        for ( int i = _bubbles.Count - 1; i >= 0; i-- )
        {
            var bubble = _bubbles[ i ];
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

    void DrawBubbles()
    {
        foreach ( var bubble in _bubbles )
        {
            DrawBubble( bubble.Position.X, bubble.Position.Y, bubble.Size );
        }
    }

    void DrawBubble( float x, float y, float size )
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

    void AdjustProjection()
    {
        float aspectRatio = ( float )Size.X / Size.Y;
        Matrix4 projection = Matrix4.CreateOrthographic( 1600 * aspectRatio, 1200, -1, 1 );
        GL.MatrixMode( MatrixMode.Projection );
        GL.LoadMatrix( ref projection );
    }

    void DrawEllipse( float x, float y, float radiusX, float radiusY, Color4 color, PrimitiveType primitiveType = PrimitiveType.Polygon )
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

    void DrawAquarium()
    {
        DrawPlants();
        DrawStones();
        DrawFish();
    }

    void DrawPlants()
    {
        DrawPlant( -500.0f, -600.0f, 60.0f, 450.0f );
        DrawPlant( -100.0f, -600.0f, 50.0f, 250.0f );
        DrawPlant( 100.0f, -600.0f, 60.0f, 300.0f );
        DrawPlant( 300.0f, -600.0f, 20.0f, 150.0f );
        DrawPlant( 500.0f, -600.0f, 60.0f, 550.0f );
        DrawPlant( 370.0f, -600.0f, 100.0f, 750.0f );
        DrawPlant( -700.0f, -600.0f, 80.0f, 350.0f );
    }

    void DrawStones()
    {
        DrawStone2( 370.0f, -600.0f, 100.0f, 100.0f );
        DrawStone4( -240.0f, -600.0f, 50.0f, 50.0f );
        DrawStone2( 100.0f, -600.0f, 60.0f, 30.0f );
        DrawStone2( 600.0f, -600.0f, 40.0f, 20.0f );
        DrawStone1( 60.0f, -600.0f, 60.0f, 40.0f );
        DrawStone3( -550.0f, -600.0f, 50.0f, 50.0f );
        DrawStone4( 200.0f, -600.0f, 40.0f, 40.0f );
        DrawStone1( 500.0f, -600.0f, 70.0f, 50.0f );
        DrawStone5( 440.0f, -600.0f, 40.0f, 40.0f );
        DrawStone5( 630.0f, -600.0f, 40.0f, 40.0f );
    }

    void DrawFish()
    {
        DrawFish1( FishBasePositions[ 0 ].X, FishBasePositions[ 0 ].Y, 120.0f, 80.0f );
        DrawFish2( FishBasePositions[ 1 ].X, FishBasePositions[ 1 ].Y, 150.0f, 40.0f );
        DrawFish3( FishBasePositions[ 2 ].X, FishBasePositions[ 2 ].Y, 100.0f, 60.0f );
        DrawFish4( FishBasePositions[ 3 ].X, FishBasePositions[ 3 ].Y, 120.0f, 80.0f );
        DrawFish5( FishBasePositions[ 4 ].X, FishBasePositions[ 4 ].Y, 100.0f, 60.0f );
    }

    void DrawPlant( float x, float y, float width, float height )
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

    void DrawFish1( float x, float y, float rx, float ry )
    {
        GL.PushMatrix();
        GL.Translate( -_fishOffsets[ 0 ], 0.0f, 0.0f );

        GL.Begin( PrimitiveType.TriangleFan );
        GL.Color3( 1.0f, 0.9f, 0.2f );
        GL.Vertex2( x, y );
        GL.Color3( 1.0f, 0.7f, 0.0f );
        for ( int i = 0; i <= 360; i += 30 )
        {
            float angle = i * ( float )Math.PI / 180;
            GL.Vertex2( x + rx * ( float )Math.Cos( angle ), y + ry * ( float )Math.Sin( angle ) );
        }
        GL.End();

        GL.Begin( PrimitiveType.TriangleFan );
        GL.Color3( 1.0f, 0.7f, 0.0f );
        GL.Vertex2( x + rx, y );
        GL.Vertex2( x + rx + 70, y + 40 );
        GL.Vertex2( x + rx + 90, y + 20 );
        GL.Vertex2( x + rx + 90, y - 20 );
        GL.Vertex2( x + rx + 70, y - 40 );
        GL.Vertex2( x + rx, y );
        GL.End();

        DrawEllipse( x - 45, y + 25, 12, 14, Color4.White );
        DrawEllipse( x - 45, y + 25, 8, 10, new Color4( 0.3f, 0.6f, 1.0f, 1.0f ) );
        DrawEllipse( x - 45, y + 25, 5, 7, Color4.Black );

        GL.Begin( PrimitiveType.Points );
        GL.PointSize( 3.0f );
        GL.Color3( 1.0f, 1.0f, 1.0f );
        GL.Vertex2( x - 42, y + 29 );
        GL.End();

        GL.PopMatrix();
    }

    void DrawFish2( float x, float y, float rx, float ry )
    {
        GL.PushMatrix();
        GL.Translate( -_fishOffsets[ 1 ], 0.0f, 0.0f );

        GL.Begin( PrimitiveType.TriangleFan );
        GL.Color3( 1.0f, 0.3f, 0.3f );
        GL.Vertex2( x, y );
        GL.Color3( 0.9f, 0.1f, 0.1f );
        for ( int i = 0; i <= 360; i += 30 )
        {
            float angle = i * ( float )Math.PI / 180;
            GL.Vertex2( x + rx * ( float )Math.Cos( angle ), y + ry * ( float )Math.Sin( angle ) );
        }
        GL.End();

        GL.Begin( PrimitiveType.Triangles );
        GL.Color3( 0.9f, 0.0f, 0.0f );
        GL.Vertex2( x + rx, y );
        GL.Vertex2( x + rx + 60, y + 50 );
        GL.Vertex2( x + rx + 40, y );

        GL.Vertex2( x + rx, y );
        GL.Vertex2( x + rx + 60, y - 50 );
        GL.Vertex2( x + rx + 40, y );
        GL.End();

        DrawEllipse( x - 35, y + 12, 6, 8, Color4.White );
        DrawEllipse( x - 33, y + 14, 3, 4, Color4.Black );

        GL.Begin( PrimitiveType.Points );
        GL.PointSize( 2.0f );
        GL.Color3( 1.0f, 1.0f, 1.0f );
        GL.Vertex2( x - 31, y + 16 );
        GL.End();

        GL.PopMatrix();
    }

    void DrawFish3( float x, float y, float rx, float ry )
    {
        GL.PushMatrix();
        GL.Translate( -_fishOffsets[ 2 ], 0.0f, 0.0f );

        GL.Begin( PrimitiveType.TriangleFan );
        GL.Color3( 0.3f, 0.5f, 1.0f );
        GL.Vertex2( x, y );
        GL.Color3( 0.1f, 0.3f, 0.9f );
        for ( int i = 0; i <= 360; i += 30 )
        {
            float angle = i * ( float )Math.PI / 180;
            GL.Vertex2( x + rx * ( float )Math.Cos( angle ), y + ry * ( float )Math.Sin( angle ) );
        }
        GL.End();

        GL.Begin( PrimitiveType.Triangles );
        GL.Color3( 0.2f, 0.4f, 0.9f );
        GL.Vertex2( x - 30, y + 55 );
        GL.Vertex2( x, y + 90 );
        GL.Vertex2( x + 30, y + 58 );

        GL.Vertex2( x - 30, y - 20 );
        GL.Vertex2( x, y - 50 );
        GL.Vertex2( x + 30, y - 20 );
        GL.End();

        GL.Begin( PrimitiveType.TriangleFan );
        GL.Color3( 0.1f, 0.3f, 0.8f );
        GL.Vertex2( x + rx, y );
        GL.Vertex2( x + rx + 80, y + 30 );
        GL.Vertex2( x + rx + 100, y );
        GL.Vertex2( x + rx + 80, y - 30 );
        GL.Vertex2( x + rx, y );
        GL.End();

        float eyeX = x - 20;
        float eyeY = y + 20;

        DrawEllipse( eyeX, eyeY, 8, 10, Color4.White );
        DrawEllipse( eyeX, eyeY, 5, 7, new Color4( 0.3f, 0.9f, 0.5f, 1.0f ) );
        DrawEllipse( eyeX, eyeY, 3, 4, Color4.Black );

        GL.Begin( PrimitiveType.Points );
        GL.PointSize( 2.0f );
        GL.Color3( 1.0f, 1.0f, 1.0f );
        GL.Vertex2( eyeX + 2, eyeY + 3 );
        GL.End();

        GL.PopMatrix();
    }

    void DrawFish4( float x, float y, float rx, float ry )
    {
        GL.PushMatrix();
        GL.Translate( -_fishOffsets[ 3 ], 0.0f, 0.0f );

        GL.Begin( PrimitiveType.TriangleFan );
        GL.Color3( 0.3f, 0.9f, 0.3f );
        GL.Vertex2( x, y );
        GL.Color3( 0.1f, 0.7f, 0.1f );
        for ( int i = 0; i <= 360; i += 30 )
        {
            float angle = i * ( float )Math.PI / 180;
            GL.Vertex2( x + rx * ( float )Math.Cos( angle ), y + ry * ( float )Math.Sin( angle ) );
        }
        GL.End();

        GL.Begin( PrimitiveType.TriangleFan );
        GL.Color3( 0.2f, 0.7f, 0.2f );
        GL.Vertex2( x + rx, y );
        for ( int i = -3; i <= 3; i++ )
        {
            float offset = i * 20;
            GL.Color3( 0.3f - i * 0.05f, 0.8f - i * 0.05f, 0.3f - i * 0.05f );
            GL.Vertex2( x + rx + 70, y + offset );
        }
        GL.Vertex2( x + rx, y );
        GL.End();

        DrawEllipse( x - 45, y + 20, 10, 12, Color4.White );
        DrawEllipse( x - 45, y + 20, 7, 9, new Color4( 1.0f, 0.9f, 0.1f, 1.0f ) );
        DrawEllipse( x - 45, y + 20, 4, 5, Color4.Black );

        GL.Begin( PrimitiveType.Points );
        GL.PointSize( 3.0f );
        GL.Color3( 1.0f, 1.0f, 1.0f );
        GL.Vertex2( x - 42, y + 24 );
        GL.End();

        GL.PopMatrix();
    }

    void DrawFish5( float x, float y, float rx, float ry )
    {
        GL.PushMatrix();

        if ( _fishOffsets[ 4 ] > 1300.0f )
        {
            _fishOffsets[ 4 ] = -600.0f;
        }

        if ( _fishOffsets[ 4 ] <= -100 && _fishOffsets[ 4 ] >= -102 )
        {
            _bubbles.Add( new Bubble( new Vector2( _fishOffsets[ 4 ] + 280.0f, y ), new Vector2( 0, 50 ), new Vector2( 0, 1 ) ) );
        }

        GL.Translate( -_fishOffsets[ 4 ], 0.0f, 0.0f );

        DrawEllipse( x, y, rx, ry, new Color4( 0.97f, 0.08f, 0.78f, 1.0f ) );

        GL.Begin( PrimitiveType.Polygon );
        GL.Color3( 0.97f, 0.08f, 0.78f );
        GL.Vertex2( x + rx, y );
        GL.Color3( 0.0f, 0.0f, 1f );
        GL.Vertex2( x + rx + FishBasePositions[ 3 ].Y, y + FishBasePositions[ 3 ].Y );
        GL.Color3( 0.0f, 0.0f, 1f );
        GL.Vertex2( x + rx + 100.0f, y + 20.0f );
        GL.Vertex2( x + rx + 70.0f, y );
        GL.Vertex2( x + rx + 100.0f, y - 20.0f );
        GL.Color3( 0.0f, 0.0f, 1f );
        GL.Vertex2( x + rx + FishBasePositions[ 3 ].Y, y - FishBasePositions[ 3 ].Y );
        GL.Color3( 0.0f, 0.0f, 1f );
        GL.Vertex2( x + rx, y );
        GL.End();

        DrawEllipse( x - FishBasePositions[ 3 ].Y, y + 20.0f, ry / 2, ry / 2, Color4.White );
        DrawEllipse( x - 40.0f, y + 30.0f, ry / 4, ry / 4, Color4.Black );

        GL.PopMatrix();
    }

    void DrawStone1( float x, float y, float width, float height )
    {
        GL.Begin( PrimitiveType.TriangleFan );
        GL.Color3( 0.3f, 0.2f, 0.1f );
        GL.Vertex2( x, y );

        for ( int i = 0; i <= 12; i++ )
        {
            float angle = i * 2 * ( float )Math.PI / 12;
            float brightness = 0.5f + 0.3f * ( float )Math.Sin( angle );
            GL.Color3( brightness * 0.6f, brightness * 0.4f, brightness * 0.2f );
            GL.Vertex2(
                x + width * ( float )Math.Cos( angle ) * 0.8f,
                y + height * ( float )Math.Sin( angle ) * 0.5f
            );
        }
        GL.End();
    }

    void DrawStone2( float x, float y, float width, float height )
    {
        GL.Begin( PrimitiveType.TriangleFan );
        GL.Color3( 0.5f, 0.5f, 0.5f );
        GL.Vertex2( x, y );

        for ( int i = 0; i <= 10; i++ )
        {
            float angle = i * 2 * ( float )Math.PI / 10;
            float shade = 0.3f + 0.5f * ( i % 3 ) * 0.3f;
            GL.Color3( 0.6f - shade, 0.6f - shade, 0.6f - shade );
            GL.Vertex2(
                x + width * ( float )Math.Cos( angle ) * 0.9f,
                y + height * ( float )Math.Sin( angle ) * 0.8f
            );
        }
        GL.End();
    }

    void DrawStone3( float x, float y, float width, float height )
    {
        GL.Begin( PrimitiveType.Polygon );

        GL.Color3( 0.4f, 0.3f, 0.2f );
        GL.Vertex2( x - width / 2, y - height / 4 );

        GL.Color3( 0.5f, 0.4f, 0.3f );
        GL.Vertex2( x - width / 3, y + height / 3 );

        GL.Color3( 0.6f, 0.5f, 0.4f );
        GL.Vertex2( x + width / 4, y + height / 2 );

        GL.Color3( 0.5f, 0.4f, 0.3f );
        GL.Vertex2( x + width / 2, y );

        GL.Color3( 0.4f, 0.3f, 0.2f );
        GL.Vertex2( x + width / 3, y - height / 2 );
        GL.End();
    }

    void DrawStone4( float x, float y, float width, float height )
    {
        GL.Begin( PrimitiveType.TriangleFan );
        GL.Color3( 0.7f, 0.6f, 0.5f );
        GL.Vertex2( x, y );

        for ( int i = 0; i <= 8; i++ )
        {
            float angle = i * 2 * ( float )Math.PI / 8;
            GL.Color3( 0.5f + 0.2f * ( i % 3 ), 0.7f + 0.2f * ( i % 3 ), 0.9f + 0.2f * ( i % 3 ) );
            GL.Vertex2(
                x + width * ( float )Math.Cos( angle ),
                y + height * ( float )Math.Sin( angle ) * 0.3f
            );
        }
        GL.End();
    }

    void DrawStone5( float x, float y, float width, float height )
    {
        GL.Begin( PrimitiveType.TriangleFan );
        GL.Color3( 0.4f, 0.3f, 0.2f );
        GL.Vertex2( x, y );

        for ( int i = 0; i <= 8; i++ )
        {
            float angle = i * 2 * ( float )Math.PI / 8;
            GL.Color3( 0.5f, 0.4f, 0.3f );
            GL.Vertex2(
                x + width * ( float )Math.Cos( angle ),
                y + height * ( float )Math.Sin( angle )
            );
        }
        GL.End();
    }
}