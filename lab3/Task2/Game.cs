using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using Task2.Components;

namespace Task2;

public class Game : GameWindow
{
    private float speed1 = 2.5f;
    private float speed2 = 2.2f;
    private float speed3 = 2.8f;
    private float speed4 = 2.0f;
    private float speed5 = 3.0f;

    private float fish1X = -1450;
    private float fish2X = -850;
    private float fish3X = -1200;
    private float fish4X = -950;
    private float fish5X = -600;

    private float bubbleTimer1 = 0f;
    private float bubbleTimer2 = 0f;
    private float bubbleTimer3 = 0f;
    private float bubbleTimer4 = 0f;
    private float bubbleTimer5 = 0f;

    private readonly List<Bubble> bubbles = new List<Bubble>();
    private readonly Random random = new Random();

    public Game( GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings )
        : base( gameWindowSettings, nativeWindowSettings )
    {
    }

    protected override void OnLoad()
    {
        base.OnLoad();
        GL.ClearColor( Color4.White );
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

        bubbleTimer1 += deltaTime;
        bubbleTimer2 += deltaTime;
        bubbleTimer3 += deltaTime;
        bubbleTimer4 += deltaTime;
        bubbleTimer5 += deltaTime;

        if ( bubbleTimer1 > 0.9 )
        {
            GenerateFishBubbles( 1, 370.0f - fish1X, -100.0f );
            bubbleTimer1 = 0f;
        }

        if ( bubbleTimer2 > 0.7f )
        {
            GenerateFishBubbles( 2, 250.0f - fish2X, 300.0f );
            bubbleTimer2 = 0f;
        }

        if ( bubbleTimer3 > 0.7f )
        {
            GenerateFishBubbles( 3, -400.0f - fish3X, -200.0f );
            bubbleTimer3 = 0f;
        }

        if ( bubbleTimer4 > 1.2f )
        {
            GenerateFishBubbles( 4, -170.0f - fish4X, 50.0f );
            bubbleTimer4 = 0f;
        }

        if ( bubbleTimer5 > 0.9f )
        {
            GenerateFishBubbles( 5, 440.0f - fish5X, -300.0f );
            bubbleTimer5 = 0f;
        }
    }

    void GenerateFishBubbles( int fishNumber, float fishX, float fishY )
    {
        int bubbleCount = 1;

        for ( int i = 0; i < bubbleCount; i++ )
        {
            float offsetX = random.Next( 10, 30 );
            float offsetY = random.Next( -20, 20 );
            float bubbleX = fishX + offsetX;
            float bubbleY = fishY + offsetY;
            float size = random.Next( 8, 15 );
            float speedY = random.Next( 40, 100 );
            float speedX = random.Next( -5, 5 );

            bubbles.Add( new Bubble(
                new Vector2( bubbleX, bubbleY ),
                new Vector2( speedX, speedY ),
                new Vector2( 0, 1 ),
                size
            ) );
        }
    }

    void UpdateFishPositions( float deltaTime )
    {
        fish1X += speed1 * deltaTime * 100;
        fish2X += speed2 * deltaTime * 100;
        fish3X += speed3 * deltaTime * 100;
        fish4X += speed4 * deltaTime * 100;
        fish5X += speed5 * deltaTime * 100;

        if ( fish1X >= 1400 )
            fish1X = -1450;
        if ( fish2X >= 1400 )
            fish2X = -1450;
        if ( fish3X >= 1400 )
            fish3X = -1450;
        if ( fish4X >= 1400 )
            fish4X = -1450;
        if ( fish5X >= 1400 )
            fish5X = -1450;
    }

    void UpdateBubbles( float deltaTime )
    {
        for ( int i = bubbles.Count - 1; i >= 0; i-- )
        {
            var bubble = bubbles[ i ];
            bubble.Position += bubble.Speed * bubble.Direction * deltaTime;
            bubble.Position.X += ( float )Math.Sin( bubble.LifeTime * 3 ) * deltaTime * 3;
            bubble.LifeTime -= deltaTime;

            if ( bubble.Position.Y > 600 ||
                bubble.Position.X > 800 ||
                bubble.Position.X < -800 ||
                bubble.LifeTime <= 0 )
            {
                bubbles.RemoveAt( i );
            }
        }
    }

    protected override void OnRenderFrame( FrameEventArgs args )
    {
        GL.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );
        DrawAquarium();
        DrawBubbles();
        SwapBuffers();
        base.OnRenderFrame( args );
    }

    void DrawBubbles()
    {
        foreach ( var bubble in bubbles )
        {
            DrawBubble( bubble.Position.X, bubble.Position.Y, bubble.Size );
        }
    }

    void DrawBubble( float x, float y, float size )
    {
        GL.PushMatrix();
        GL.Translate( x, y, 0 );

        // Основной пузырек (полупрозрачный)
        DrawEllipse( 0, 0, size, size, new Color4( 1.0f, 1.0f, 1.0f, 0.4f ) );

        // Контур пузырька
        DrawEllipse( 0, 0, size, size, new Color4( 1.0f, 1.0f, 1.0f, 0.8f ), PrimitiveType.LineLoop );

        // Блик на пузырьке
        GL.Begin( PrimitiveType.Points );
        GL.Color3( 1.0f, 1.0f, 1.0f );
        GL.Vertex2( size * 0.3f, size * 0.3f );
        GL.End();

        GL.PopMatrix();
    }

    protected override void OnUnload()
    {
        base.OnUnload();
        GL.ClearColor( Color4.White );
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

        int segments = primitiveType == PrimitiveType.LineLoop ? 360 : 360;

        for ( int i = 0; i < segments; i++ )
        {
            float degInRad = i * ( float )Math.PI / 180;
            GL.Vertex2( x + Math.Cos( degInRad ) * radiusX, y + Math.Sin( degInRad ) * radiusY );
        }
        GL.End();
    }

    void DrawAquarium()
    {
        GL.Begin( PrimitiveType.Quads );
        GL.Color3( 0.6f, 0.77f, 0.57f );
        GL.Vertex2( -800.0f, -600.0f );
        GL.Vertex2( 800.0f, -600.0f );
        GL.Vertex2( 800.0f, 600.0f );
        GL.Vertex2( -800.0f, 600.0f );
        GL.End();

        DrawPlant( -500.0f, -600.0f, 60.0f, 450.0f );
        DrawPlant( -100.0f, -600.0f, 50.0f, 250.0f );
        DrawPlant( 100.0f, -600.0f, 60.0f, 300.0f );
        DrawPlant( 300.0f, -600.0f, 20.0f, 150.0f );
        DrawPlant( 500.0f, -600.0f, 60.0f, 550.0f );
        DrawPlant( -400.0f, -600.0f, 100.0f, 750.0f );
        DrawPlant( -700.0f, -600.0f, 80.0f, 350.0f );

        DrawStone2( -400.0f, -600.0f, 100.0f, 100.0f );
        DrawStone4( -240.0f, -600.0f, 50.0f, 50.0f );
        DrawStone2( 100.0f, -600.0f, 60.0f, 30.0f );
        DrawStone2( 600.0f, -600.0f, 40.0f, 20.0f );
        DrawStone1( 60.0f, -600.0f, 60.0f, 40.0f );
        DrawStone3( -550.0f, -600.0f, 50.0f, 50.0f );
        DrawStone4( 200.0f, -600.0f, 40.0f, 40.0f );
        DrawStone1( 500.0f, -600.0f, 70.0f, 50.0f );
        DrawStone5( 440.0f, -600.0f, 40.0f, 40.0f );
        DrawStone5( 630.0f, -600.0f, 40.0f, 40.0f );

        DrawFish1( 370.0f, -100.0f, 120.0f, 80.0f );
        DrawFish2( 250.0f, 300.0f, 150.0f, 40.0f );
        DrawFish3( -400.0f, -200.0f, 100.0f, 60.0f );
        DrawFish4( -170.0f, 50.0f, 120.0f, 80.0f );
        DrawFish5( 440.0f, -300.0f, 100.0f, 60.0f );
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
        GL.Translate( -fish1X, 0.0f, 0.0f );

        // Тело с градиентом
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

        // Хвост
        GL.Begin( PrimitiveType.TriangleFan );
        GL.Color3( 1.0f, 0.7f, 0.0f );
        GL.Vertex2( x + rx, y );
        GL.Vertex2( x + rx + 70, y + 40 );
        GL.Vertex2( x + rx + 90, y + 20 );
        GL.Vertex2( x + rx + 90, y - 20 );
        GL.Vertex2( x + rx + 70, y - 40 );
        GL.Vertex2( x + rx, y );
        GL.End();

        // Большой глаз
        DrawEllipse( x - 45, y + 25, 12, 14, Color4.White );
        DrawEllipse( x - 45, y + 25, 8, 10, new Color4( 0.3f, 0.6f, 1.0f, 1.0f ) );
        DrawEllipse( x - 45, y + 25, 5, 7, Color4.Black );

        // Блик
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
        GL.Translate( -fish2X, 0.0f, 0.0f );

        // Тело
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

        // Хвост (двойной)
        GL.Begin( PrimitiveType.Triangles );
        GL.Color3( 0.9f, 0.0f, 0.0f );
        GL.Vertex2( x + rx, y );
        GL.Vertex2( x + rx + 60, y + 50 );
        GL.Vertex2( x + rx + 40, y );

        GL.Vertex2( x + rx, y );
        GL.Vertex2( x + rx + 60, y - 50 );
        GL.Vertex2( x + rx + 40, y );
        GL.End();

        // Маленький глаз
        DrawEllipse( x - 35, y + 12, 6, 8, Color4.White );
        DrawEllipse( x - 33, y + 14, 3, 4, Color4.Black );

        // Блик
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
        GL.Translate( -fish3X, 0.0f, 0.0f );

        // Тело
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

        // Плавники
        GL.Begin( PrimitiveType.Triangles );
        GL.Color3( 0.2f, 0.4f, 0.9f );
        GL.Vertex2( x - 30, y + 55 );
        GL.Vertex2( x, y + 90 );
        GL.Vertex2( x + 30, y + 58 );

        GL.Vertex2( x - 30, y - 20 );
        GL.Vertex2( x, y - 50 );
        GL.Vertex2( x + 30, y - 20 );
        GL.End();

        // Хвост
        GL.Begin( PrimitiveType.TriangleFan );
        GL.Color3( 0.1f, 0.3f, 0.8f );
        GL.Vertex2( x + rx, y );
        GL.Vertex2( x + rx + 80, y + 30 );
        GL.Vertex2( x + rx + 100, y );
        GL.Vertex2( x + rx + 80, y - 30 );
        GL.Vertex2( x + rx, y );
        GL.End();

        // Глаза навыкате (два глаза)
        for ( int side = -1; side <= 1; side += 2 )
        {
            float eyeX = x - 20 + side * 15;
            float eyeY = y + 20;

            DrawEllipse( eyeX, eyeY, 8, 10, Color4.White );
            DrawEllipse( eyeX, eyeY, 5, 7, new Color4( 0.3f, 0.9f, 0.5f, 1.0f ) );
            DrawEllipse( eyeX, eyeY, 3, 4, Color4.Black );

            // Блик
            GL.Begin( PrimitiveType.Points );
            GL.PointSize( 2.0f );
            GL.Color3( 1.0f, 1.0f, 1.0f );
            GL.Vertex2( eyeX + 2, eyeY + 3 );
            GL.End();
        }

        GL.PopMatrix();
    }

    void DrawFish4( float x, float y, float rx, float ry )
    {
        GL.PushMatrix();
        GL.Translate( -fish4X, 0.0f, 0.0f );

        // Тело
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

        // Хвост (веером)
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

        // Левый глаз (большой)
        DrawEllipse( x - 45, y + 20, 10, 12, Color4.White );
        DrawEllipse( x - 45, y + 20, 7, 9, new Color4( 1.0f, 0.9f, 0.1f, 1.0f ) );
        DrawEllipse( x - 45, y + 20, 4, 5, Color4.Black );

        // Блик
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

        if ( fish5X > 1300.0f )
        {
            fish5X = -600.0f;
        }

        if ( fish5X <= -100 && fish5X >= -102 )
        {
            bubbles.Add( new Bubble( new Vector2( fish5X + 280.0f, y ), new Vector2( 0, 50 ), new Vector2( 0, 1 ) ) );
        }

        GL.Translate( -fish5X, 0.0f, 0.0f );

        // Тело
        DrawEllipse( x, y, rx, ry, new Color4( 0.97f, 0.08f, 0.78f, 1.0f ) );

        // Хвост
        GL.Begin( PrimitiveType.Polygon );
        GL.Color3( 0.97f, 0.08f, 0.78f );
        GL.Vertex2( x + rx, y );
        GL.Color3( 0.0f, 0.0f, 1f );
        GL.Vertex2( x + rx + 50.0f, y + 50.0f );
        GL.Color3( 0.0f, 0.0f, 1f );
        GL.Vertex2( x + rx + 100.0f, y + 20.0f );
        GL.Vertex2( x + rx + 70.0f, y );
        GL.Vertex2( x + rx + 100.0f, y - 20.0f );
        GL.Color3( 0.0f, 0.0f, 1f );
        GL.Vertex2( x + rx + 50.0f, y - 50.0f );
        GL.Color3( 0.0f, 0.0f, 1f );
        GL.Vertex2( x + rx, y );
        GL.End();

        // Глаз
        DrawEllipse( x - 50.0f, y + 20.0f, ry / 2, ry / 2, Color4.White );
        DrawEllipse( x - 40.0f, y + 30.0f, ry / 4, ry / 4, Color4.Black );

        GL.PopMatrix();
    }

    // Камень 1: Простой овальный камень с градиентом
    void DrawStone1( float x, float y, float width, float height )
    {
        GL.Begin( PrimitiveType.TriangleFan );
        GL.Color3( 0.2f, 0.1f, 0.05f );
        GL.Vertex2( x, y - height / 2 );

        for ( int i = 0; i <= 20; i++ )
        {
            float angle = i * 2 * ( float )Math.PI / 20;
            float intensity = 0.3f + 0.4f * ( float )Math.Sin( angle );
            GL.Color3( 0.5f * intensity, 0.3f * intensity, 0.1f * intensity );
            GL.Vertex2(
                x + width * ( float )Math.Cos( angle ) * 0.8f,
                y + height * ( float )Math.Sin( angle ) * 0.6f
            );
        }
        GL.End();
    }

    // Камень 2: Многоугольник с шероховатой поверхностью
    void DrawStone2( float x, float y, float width, float height )
    {
        Random rand = new Random( 123 );

        GL.Begin( PrimitiveType.TriangleFan );
        GL.Color3( 0.6f, 0.5f, 0.4f );
        GL.Vertex2( x, y );

        for ( int i = 0; i <= 12; i++ )
        {
            float angle = i * 2 * ( float )Math.PI / 12;
            float noiseX = ( float )( rand.NextDouble() - 0.5 ) * width * 0.2f;
            float noiseY = ( float )( rand.NextDouble() - 0.5 ) * height * 0.2f;

            GL.Color3( 0.5f + 0.3f * ( float )Math.Sin( angle ),
                     0.4f + 0.2f * ( float )Math.Sin( angle + 1 ),
                     0.3f + 0.1f * ( float )Math.Sin( angle + 2 ) );

            GL.Vertex2(
                x + width * ( float )Math.Cos( angle ) + noiseX,
                y + height * ( float )Math.Sin( angle ) * 0.7f + noiseY
            );
        }
        GL.End();
    }

    // Камень 3: Кристаллическая структура
    void DrawStone3( float x, float y, float width, float height )
    {
        GL.Begin( PrimitiveType.Polygon );
        GL.Color3( 0.5f, 0.6f, 0.7f );
        GL.Vertex2( x - width / 2, y );
        GL.Color3( 0.7f, 0.8f, 0.9f );
        GL.Vertex2( x - width / 3, y + height / 2 );
        GL.Color3( 0.8f, 0.9f, 1.0f );
        GL.Vertex2( x + width / 3, y + height / 2 );
        GL.Color3( 0.6f, 0.7f, 0.8f );
        GL.Vertex2( x + width / 2, y );
        GL.Color3( 0.4f, 0.5f, 0.6f );
        GL.Vertex2( x, y - height / 3 );
        GL.End();

        GL.Begin( PrimitiveType.Lines );
        GL.Color3( 1.0f, 1.0f, 1.0f );
        GL.Vertex2( x - width / 4, y + height / 4 );
        GL.Vertex2( x - width / 6, y + height / 6 );
        GL.End();
    }

    // Камень 4: Слоистый камень
    void DrawStone4( float x, float y, float width, float height )
    {
        GL.Begin( PrimitiveType.TriangleFan );
        GL.Color3( 0.4f, 0.25f, 0.15f );
        GL.Vertex2( x, y );

        for ( int i = 0; i <= 24; i++ )
        {
            float angle = i * 2 * ( float )Math.PI / 24;
            GL.Color3( 0.5f, 0.3f, 0.2f );
            GL.Vertex2(
                x + width * ( float )Math.Cos( angle ) * 0.9f,
                y + height * ( float )Math.Sin( angle ) * 0.5f
            );
        }
        GL.End();

        GL.Begin( PrimitiveType.TriangleFan );
        GL.Color3( 0.7f, 0.5f, 0.3f );
        GL.Vertex2( x, y + height / 4 );

        for ( int i = 0; i <= 16; i++ )
        {
            float angle = i * 2 * ( float )Math.PI / 16;
            GL.Color3( 0.8f, 0.6f, 0.4f );
            GL.Vertex2(
                x + width * ( float )Math.Cos( angle ) * 0.7f,
                y + height / 4 + height * ( float )Math.Sin( angle ) * 0.3f
            );
        }
        GL.End();
    }

    // Камень 5: "Драгоценный" камень с отражениями
    void DrawStone5( float x, float y, float width, float height )
    {
        GL.Begin( PrimitiveType.TriangleFan );
        GL.Color3( 0.9f, 0.8f, 0.7f );
        GL.Vertex2( x, y );

        for ( int i = 0; i <= 8; i++ )
        {
            float angle = i * 2 * ( float )Math.PI / 8;
            float r = 0.3f + 0.7f * ( float )Math.Abs( Math.Cos( angle * 2 ) );

            GL.Color3( 0.9f * r, 0.7f * r, 0.5f * r );
            GL.Vertex2(
                x + width * ( float )Math.Cos( angle ),
                y + height * ( float )Math.Sin( angle ) * 0.8f
            );
        }
        GL.End();

        GL.Begin( PrimitiveType.Polygon );
        GL.Color3( 1.0f, 1.0f, 1.0f );
        GL.Vertex2( x - width / 3, y - height / 4 );
        GL.Vertex2( x, y - height / 6 );
        GL.Vertex2( x + width / 6, y - height / 8 );
        GL.Vertex2( x - width / 8, y - height / 3 );
        GL.End();

        GL.Begin( PrimitiveType.Points );
        GL.Color3( 1.0f, 1.0f, 1.0f );
        GL.Vertex2( x - width / 4, y - height / 5 );
        GL.Vertex2( x + width / 5, y - height / 6 );
        GL.End();
    }
}