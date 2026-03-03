using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using Task2.Tools;

namespace Task2.Strategies.FishDrawingStrategies;

public class Fish2DrawingStrategy : FishDrawingStrategy
{
    public override void DrawBody(float x, float y, float rx, float ry)
    {
        GL.Begin(PrimitiveType.TriangleFan);
        GL.Color3(1.0f, 0.3f, 0.3f);
        GL.Vertex2(x, y);
        GL.Color3(0.9f, 0.1f, 0.1f);
        
        for (int i = 0; i <= 360; i += 30)
        {
            float angle = i * (float)Math.PI / 180;
            GL.Vertex2(x + rx * (float)Math.Cos(angle), y + ry * (float)Math.Sin(angle));
        }
        GL.End();
        
        DrawPectoralFins(x, y, rx);
    }

    public override void DrawTail(float x, float y, float rx)
    {
        GL.Begin(PrimitiveType.Triangles);
        GL.Color3(0.9f, 0.0f, 0.0f);
        
        GL.Vertex2(x + rx, y);
        GL.Vertex2(x + rx + 60, y + 50);
        GL.Vertex2(x + rx + 40, y);

        GL.Vertex2(x + rx, y);
        GL.Vertex2(x + rx + 60, y - 50);
        GL.Vertex2(x + rx + 40, y);
        GL.End();
    }

    public override void DrawEye(float x, float y)
    {
        float eyeX = x - 35;
        float eyeY = y + 12;
        
        DrawingTools.DrawEllipse(eyeX, eyeY, 6, 8, Color4.White);
        DrawingTools.DrawEllipse(eyeX + 2, eyeY + 2, 3, 4, Color4.Black);
        
        DrawEyeHighlight(eyeX, eyeY);
    }

    private void DrawEyeHighlight(float eyeX, float eyeY)
    {
        GL.Begin(PrimitiveType.Points);
        GL.PointSize(2.0f);
        GL.Color3(1.0f, 1.0f, 1.0f);
        GL.Vertex2(eyeX + 4, eyeY + 4);
        GL.End();
    }
    
    private void DrawPectoralFins(float x, float y, float rx)
    {
        GL.Begin(PrimitiveType.Triangles);
        GL.Color3(0.9f, 0.0f, 0.0f);
        
        GL.Vertex2(x , y + 20);
        GL.Vertex2(x + 20, y);
        GL.Vertex2(x, y - 20);
        
        GL.End();
    }
}