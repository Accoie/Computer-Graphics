using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Task1;

public class BezierGameWindow(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
    : GameWindow(gameWindowSettings, nativeWindowSettings)
{
    private const float MinX = -4.0f;
    private const float MaxX = 4.0f;
    private const float MinY = -4.0f;
    private const float MaxY = 4.0f;
    private const float Step = 0.02f;

    private readonly Point[] _bezierPoints = // какого порядка прямая
    [
        new(-2.0f, 1.5f),
        new(1.0f, 3.5f),
        new(2.0f, -1.0f),
        new(2.0f, 2.0f)
    ];

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

    protected override void OnUpdateFrame( FrameEventArgs args )
    {
        base.OnUpdateFrame( args );

        if ( KeyboardState.IsKeyDown( Keys.Escape ) )
        {
            Close();
        }
    }

    protected override void OnResize( ResizeEventArgs e )
    {
        base.OnResize( e );

        GL.Viewport( 0, 0, ClientSize.X, ClientSize.Y );
    }

    private void SetupProjection()
    {
        GL.MatrixMode( MatrixMode.Projection );
        GL.LoadIdentity();

        float aspect = ( float )ClientSize.X / ClientSize.Y;

        if ( aspect > 1 )
        {
            GL.Ortho( MinX * aspect, MaxX * aspect, MinY * aspect, MaxY * aspect, -1, 1 );
        }
        else
        {
            GL.Ortho( MinX / aspect, MaxX / aspect, MinY / aspect, MaxY / aspect, -1, 1 );
        }
    }
    
    private void DrawBezierCurve()
    {
        GL.Begin( PrimitiveType.LineStrip );
        GL.Color3( 0.0f, 0.0f, 1.0f );

        for ( float t = 0; t <= 1; t += Step )
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
        Point a = Lerp( _bezierPoints[ 0 ], _bezierPoints[ 1 ], t );
        Point b = Lerp( _bezierPoints[ 1 ], _bezierPoints[ 2 ], t );
        Point c = Lerp( _bezierPoints[ 2 ], _bezierPoints[ 3 ], t );

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

        foreach ( Point point in _bezierPoints )
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

        for ( int i = 0; i < _bezierPoints.Length - 1; i++ )
        {
            GL.Vertex2( _bezierPoints[ i ].X, _bezierPoints[ i ].Y );
            GL.Vertex2( _bezierPoints[ i + 1 ].X, _bezierPoints[ i + 1 ].Y );
        }
        
        GL.End();

        GL.Disable( EnableCap.LineStipple );
    }

    private void DrawAxes()
    {
        GL.Begin( PrimitiveType.Lines );

        GL.Color3( 0.2f, 0.2f, 0.2f );
        GL.Vertex2( MinX, 0 );
        GL.Vertex2( MaxX, 0 );
        GL.Vertex2( 0, MinY );
        GL.Vertex2( 0, MaxY );

        GL.Color3( 0.5f, 0.5f, 0.5f );
        for ( int i = ( int )MinX; i <= MaxX; i++ )
        {
            if ( i != 0 )
            {
                GL.Vertex2( i, -0.1f );
                GL.Vertex2( i, 0.1f );
            }
        }

        for ( int i = ( int )MinY; i <= MaxY; i++ )
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