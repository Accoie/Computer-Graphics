using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace Task1;

public class Point
{
    public float X { get; set; }
    public float Y { get; set; }

    public Point( float x, float y )
    {
        X = x;
        Y = y;
    }
}

public class BezierGameWindow : GameWindow
{
    // Границы для отображения (специально подобраны, чтобы вместить все точки)
    private const float minX = -2.5f;
    private const float maxX = 3.5f;
    private const float minY = -6.0f;
    private const float maxY = 6.0f;
    private const float step = 0.02f;

    private readonly Point[] bezierPoints =
    [
        new Point(-1.0f, 1.5f),
        new Point(0.0f, 2.5f),
        new Point(1.0f, -1.0f),
        new Point(2.0f, 2.0f)
    ];

    public BezierGameWindow( GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings )
        : base( gameWindowSettings, nativeWindowSettings )
    {
    }

    protected override void OnLoad()
    {
        base.OnLoad();

        GL.ClearColor( 1.0f, 1.0f, 1.0f, 1.0f );
        GL.Enable( EnableCap.LineSmooth );
        GL.Enable( EnableCap.PointSmooth );
        GL.Hint( HintTarget.LineSmoothHint, HintMode.Nicest );
        GL.Hint( HintTarget.PointSmoothHint, HintMode.Nicest );
        GL.Disable( EnableCap.DepthTest );
    }

    protected override void OnRenderFrame( FrameEventArgs args )
    {
        base.OnRenderFrame( args );

        GL.Clear( ClearBufferMask.ColorBufferBit );

        SetupProjection();

        DrawAxes();
        DrawBezierCurve();

        SwapBuffers();
    }

    private void SetupProjection()
    {
        GL.MatrixMode( MatrixMode.Projection );
        GL.LoadIdentity();

        float aspect = ( float )Size.X / Size.Y ;

        if ( aspect > 1 ) 
        {
            GL.Ortho( minX * aspect, maxX * aspect, minY, maxY, -1, 1 );
        }
        else 
        {
            GL.Ortho( minX, maxX, minY / aspect, maxY / aspect, -1, 1 );
        }

        GL.MatrixMode( MatrixMode.Modelview );
        GL.LoadIdentity();
    }

    protected override void OnUpdateFrame( FrameEventArgs args )
    {
        base.OnUpdateFrame( args );

        // Выход по нажатию ESC
        if ( KeyboardState.IsKeyDown( OpenTK.Windowing.GraphicsLibraryFramework.Keys.Escape ) )
        {
            Close();
        }
    }

    protected override void OnResize( ResizeEventArgs e )
    {
        base.OnResize( e );

        GL.Viewport( 0, 0, Size.X, Size.Y );
    }

    private void DrawBezierCurve()
    {
        // Рисуем кривую Безье
        GL.Begin( PrimitiveType.LineStrip );
        GL.Color3( 0.0f, 0.0f, 1.0f ); // Синий

        for ( float t = 0; t <= 1; t += step )
        {
            Point p = CalculateBezierPoint( t );
            GL.Vertex2( p.X, p.Y );
        }
        GL.End();

        DrawControlPoints();
        DrawDashedControlLines();
    }

    private Point CalculateBezierPoint( float t )
    {
        Point a = Lerp( bezierPoints[ 0 ], bezierPoints[ 1 ], t );
        Point b = Lerp( bezierPoints[ 1 ], bezierPoints[ 2 ], t );
        Point c = Lerp( bezierPoints[ 2 ], bezierPoints[ 3 ], t );

        Point d = Lerp( a, b, t );
        Point e = Lerp( b, c, t );

        return Lerp( d, e, t );
    }

    private Point Lerp( Point p1, Point p2, float t )
    {
        return new Point(
            p1.X + ( p2.X - p1.X ) * t,
            p1.Y + ( p2.Y - p1.Y ) * t
        );
    }

    private void DrawControlPoints()
    {
        GL.PointSize( 8.0f );
        GL.Begin( PrimitiveType.Points );
        GL.Color3( 0.0f, 0.5f, 0.9f ); // Голубой

        foreach ( var point in bezierPoints )
        {
            GL.Vertex2( point.X, point.Y );
        }
        GL.End();
        GL.PointSize( 1.0f );
    }

    private void DrawDashedControlLines()
    {
        GL.Enable( EnableCap.LineStipple );
        GL.LineStipple( 1, 0x00FF ); // Пунктир

        GL.Begin( PrimitiveType.Lines );
        GL.Color3( 0.5f, 0.5f, 0.5f ); // Серый

        for ( int i = 0; i < bezierPoints.Length - 1; i++ )
        {
            GL.Vertex2( bezierPoints[ i ].X, bezierPoints[ i ].Y );
            GL.Vertex2( bezierPoints[ i + 1 ].X, bezierPoints[ i + 1 ].Y );
        }
        GL.End();

        GL.Disable( EnableCap.LineStipple );
    }

    private void DrawAxes()
    {
        GL.Begin( PrimitiveType.Lines );

        // Оси координат
        GL.Color3( 0.2f, 0.2f, 0.2f );
        GL.Vertex2( minX, 0 );
        GL.Vertex2( maxX, 0 );
        GL.Vertex2( 0, minY );
        GL.Vertex2( 0, maxY );

        // Деления на оси X
        GL.Color3( 0.5f, 0.5f, 0.5f );
        for ( int i = ( int )minX; i <= maxX; i++ )
        {
            if ( i != 0 )
            {
                GL.Vertex2( i, -0.1f );
                GL.Vertex2( i, 0.1f );
            }
        }

        // Деления на оси Y
        for ( int i = ( int )minY; i <= maxY; i++ )
        {
            if ( i != 0 )
            {
                GL.Vertex2( -0.1f, i );
                GL.Vertex2( 0.1f, i );
            }
        }
        GL.End();
    }
}

internal class Program
{
    static void Main( string[] args )
    {
        var gameWindowSettings = GameWindowSettings.Default;

        var nativeWindowSettings = new NativeWindowSettings
        {
            ClientSize = new Vector2i( 1000, 800 ),
            Title = "Bezier Curve",
            APIVersion = new Version( 2, 1 ),
            Profile = ContextProfile.Any,
            Flags = ContextFlags.Default
        };

        using ( var gameWindow = new BezierGameWindow( gameWindowSettings, nativeWindowSettings ) )
        {
            gameWindow.Run();
        }
    }
}