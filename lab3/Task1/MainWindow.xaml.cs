using System.Windows;
using SharpGL;
using SharpGL.WPF;

namespace Task1;

public partial class MainWindow : Window
{
    private const float minX = -1.5f;
    private const float maxX = 2.5f;
    private const float minY = -3f;
    private const float maxY = 3f;
    private const float step = 0.05f;

    private readonly Point[] bezierPoints =
    [
        new Point(-1.0f, 1.5f),
        new Point(0.0f, 2.5f),
        new Point(1.0f, -1.0f),
        new Point(2.0f, 2.0f)
    ];

    public MainWindow()
    {
        InitializeComponent();
        openGLControl.OpenGLDraw += OpenGLControl_OpenGLDraw;
        openGLControl.OpenGLInitialized += OpenGLControl_OpenGLInitialized;
    }

    private void OpenGLControl_OpenGLInitialized( object sender, OpenGLRoutedEventArgs args )
    {
        var gl = openGLControl.OpenGL;
        gl.ClearColor( 1.0f, 1.0f, 1.0f, 1.0f );
    }

    private void OpenGLControl_OpenGLDraw( object sender, OpenGLRoutedEventArgs args )
    {
        var gl = openGLControl.OpenGL;
        gl.Clear( OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT );
        gl.LoadIdentity();

        var width = openGLControl.ActualWidth;
        var height = openGLControl.ActualHeight;

        float scaleX = ( float )( 1.5 / ( maxX - minX ) );
        float scaleY = ( float )( 1.5 / ( maxY - minY ) );
        float scale = Math.Min( scaleX, scaleY );

        gl.MatrixMode( OpenGL.GL_PROJECTION );
        gl.LoadIdentity();

        gl.Scale( scale, scale, 1 );

        DrawAxes( gl );
        DrawBezierCurve( gl );

        gl.Flush();
    }

    private void DrawBezierCurve( OpenGL gl )
    {
        gl.Begin( OpenGL.GL_LINE_STRIP );
        gl.Color( 0.0f, 0.0f, 1.0f );

        for ( float t = 0; t <= 1; t += step )
        {
            Point p = CalculateBezierPoint( t );
            gl.Vertex( p.X, p.Y, 0 );
        }

        gl.End();

        DrawControlPoints( gl );
        DrawDashedControlLines( gl );
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

    private void DrawControlPoints( OpenGL gl )
    {
        gl.PointSize( 8.0f );
        gl.Begin( OpenGL.GL_POINTS );
        gl.Color( 0.0f, 0.5f, 0.9f );

        foreach ( var point in bezierPoints )
        {
            gl.Vertex( point.X, point.Y, 0 );
        }

        gl.End();
        gl.PointSize( 1.0f );
    }

    private void DrawDashedControlLines( OpenGL gl )
    {
        gl.Enable( OpenGL.GL_LINE_STIPPLE );
        gl.LineStipple( 1, 0x00FF );

        gl.Begin( OpenGL.GL_LINES );
        gl.Color( 0.5f, 0.5f, 0.5f );

        for ( int i = 0; i < bezierPoints.Length - 1; i++ )
        {
            gl.Vertex( bezierPoints[ i ].X, bezierPoints[ i ].Y, 0 );
            gl.Vertex( bezierPoints[ i + 1 ].X, bezierPoints[ i + 1 ].Y, 0 );
        }

        gl.End();

        gl.Disable( OpenGL.GL_LINE_STIPPLE );
    }

    private void DrawAxes( OpenGL gl )
    {
        gl.Begin( OpenGL.GL_LINES );

        gl.Color( 0.2f, 0.2f, 0.2f );
        gl.Vertex( minX, 0, 0 );
        gl.Vertex( maxX, 0, 0 );

        gl.Vertex( 0, minY, 0 );
        gl.Vertex( 0, maxY, 0 );

        gl.Color( 0.5f, 0.5f, 0.5f );

        for ( int i = ( int )minX; i <= maxX; i++ )
        {
            if ( i != 0 )
            {
                gl.Vertex( i, -0.05f, 0 );
                gl.Vertex( i, 0.05f, 0 );
            }
        }

        for ( int i = ( int )minY; i <= maxY; i++ )
        {
            if ( i != 0 )
            {
                gl.Vertex( -0.05f, i, 0 );
                gl.Vertex( 0.05f, i, 0 );
            }
        }

        gl.End();
    }
}