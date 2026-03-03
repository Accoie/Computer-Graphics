using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using Task2.Tools;

namespace Task2.Strategies.FishDrawingStrategies;

public class Fish5DrawingStrategy : FishDrawingStrategy
{
    public override void DrawBody(float x, float y, float rx, float ry)
    {
        DrawingTools.DrawEllipse(x, y, rx, ry, new Color4(0.97f, 0.08f, 0.78f, 1.0f));
        
        DrawPectoralFins(x, y, rx);
    }

    public override void DrawTail(float x, float y, float rx)
    {
        GL.Begin(PrimitiveType.Polygon);
        GL.Color3(0.97f, 0.08f, 0.78f);
        GL.Vertex2(x + rx, y);
        
        GL.Color3(0.0f, 0.0f, 1f);
        GL.Vertex2(x + rx + 50, y + 50);
        GL.Vertex2(x + rx + 100.0f, y + 20.0f);
        GL.Vertex2(x + rx + 70.0f, y);
        GL.Vertex2(x + rx + 100.0f, y - 20.0f);
        GL.Vertex2(x + rx + 50, y - 50);
        
        GL.Color3(0.97f, 0.08f, 0.78f);
        GL.Vertex2(x + rx, y);
        GL.End();
    }

    public override void DrawEye(float x, float y)
    {
        float eyeX = x - 50;
        float eyeY = y + 20.0f;
        float ry = Fish.Size.Y; 
        
        DrawingTools.DrawEllipse(eyeX, eyeY, ry / 2, ry / 2, Color4.White);
        DrawingTools.DrawEllipse(x - 40.0f, y + 30.0f, ry / 4, ry / 4, Color4.Black);
    }
    
    private void DrawPectoralFins(float x, float y, float rx)
    {
        GL.Begin(PrimitiveType.Triangles);
        GL.Color3(0.0f, 0.0f, 0.8f);
        
        GL.Vertex2(x + 10 , y + 20);
        GL.Vertex2(x + 40, y);
        GL.Vertex2(x + 10, y - 20);
        
        GL.End();
    }
}