using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace Task1;

public class BezierGameWindow : GameWindow
{
    private const float minX = -4.0f;
    private const float maxX = 4.0f;
    private const float minY = -4.0f;
    private const float maxY = 4.0f;
    private const float step = 0.02f;

    private readonly Point[] bezierPoints =
    [
        new Point(-2.0f, 1.5f),
        new Point(1.0f, 3.5f),
        new Point(2.0f, -1.0f),
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

        float aspect = ( float )ClientSize.X / ClientSize.Y;

        if ( aspect > 1 )
        {
            GL.Ortho( minX * aspect, maxX * aspect, minY * aspect, maxY * aspect, -1, 1 );
        }
        else
        {
            GL.Ortho( minX / aspect, maxX / aspect, minY / aspect, maxY / aspect, -1, 1 );
        }

        GL.MatrixMode( MatrixMode.Modelview );
        GL.LoadIdentity();
    }

    protected override void OnUpdateFrame( FrameEventArgs args )
    {
        base.OnUpdateFrame( args );

        if ( KeyboardState.IsKeyDown( OpenTK.Windowing.GraphicsLibraryFramework.Keys.Escape ) )
        {
            Close();
        }
    }

    protected override void OnResize( ResizeEventArgs e )
    {
        base.OnResize( e );

        GL.Viewport( 0, 0, ClientSize.X, ClientSize.Y );
    }

    private void DrawBezierCurve()
    {
        GL.Begin( PrimitiveType.LineStrip );
        GL.Color3( 0.0f, 0.0f, 1.0f );

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
        GL.Color3( 0.0f, 0.5f, 0.9f );

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
        GL.LineStipple( 1, 0x00FF );

        GL.Begin( PrimitiveType.Lines );
        GL.Color3( 0.5f, 0.5f, 0.5f );

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

        GL.Color3( 0.2f, 0.2f, 0.2f );
        GL.Vertex2( minX, 0 );
        GL.Vertex2( maxX, 0 );
        GL.Vertex2( 0, minY );
        GL.Vertex2( 0, maxY );

        GL.Color3( 0.5f, 0.5f, 0.5f );
        for ( int i = ( int )minX; i <= maxX; i++ )
        {
            if ( i != 0 )
            {
                GL.Vertex2( i, -0.1f );
                GL.Vertex2( i, 0.1f );
            }
        }

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
