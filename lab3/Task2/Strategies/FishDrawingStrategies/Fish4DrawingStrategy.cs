using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using Task2.Tools;

namespace Task2.Strategies.FishDrawingStrategies;

public class Fish4DrawingStrategy : FishDrawingStrategy
{
    public override void DrawBody(float x, float y, float rx, float ry)
    {
        GL.Begin(PrimitiveType.TriangleFan);
        GL.Color3(0.3f, 0.9f, 0.3f);
        GL.Vertex2(x, y);
        GL.Color3(0.1f, 0.7f, 0.1f);
        
        for (int i = 0; i <= 360; i += 30)
        {
            float angle = i * (float)Math.PI / 180;
            GL.Vertex2(x + rx * (float)Math.Cos(angle), y + ry * (float)Math.Sin(angle));
        }

        
        GL.End();
        DrawDorsalFin(x, y + 30, rx);
        DrawPectoralFins(x + 20, y + 10, rx);
    }

    public override void DrawTail(float x, float y, float rx)
    {
        GL.Begin(PrimitiveType.TriangleFan);
        GL.Color3(0.2f, 0.7f, 0.2f);
        GL.Vertex2(x + rx, y);
        
        for (int i = -3; i <= 3; i++)
        {
            float offset = i * 20;
            float colorShift = i * 0.05f;
            GL.Color3(0.3f - colorShift, 0.8f - colorShift, 0.3f - colorShift);
            GL.Vertex2(x + rx + 70, y + offset);
        }
        
        GL.Vertex2(x + rx, y);
        GL.End();
    }

    public override void DrawEye(float x, float y)
    {
        float eyeX = x - 45;
        float eyeY = y + 20;
        
        DrawingTools.DrawEllipse(eyeX, eyeY, 10, 12, Color4.White);
        DrawingTools.DrawEllipse(eyeX, eyeY, 7, 9, new Color4(1.0f, 0.9f, 0.1f, 1.0f));
        DrawingTools.DrawEllipse(eyeX, eyeY, 4, 5, Color4.Black);
        
        DrawEyeHighlight(eyeX, eyeY);
    }

    private void DrawEyeHighlight(float eyeX, float eyeY)
    {
        GL.Begin(PrimitiveType.Points);
        GL.PointSize(3.0f);
        GL.Color3(1.0f, 1.0f, 1.0f);
        GL.Vertex2(eyeX + 3, eyeY + 4);
        GL.End();
    }

    private void DrawDorsalFin(float x, float y, float rx)
    {
        GL.Begin(PrimitiveType.Triangles);
        GL.Color3(0.15f, 0.6f, 0.15f);
        
        GL.Vertex2(x - 30, y + 40);
        GL.Vertex2(x - 10, y + 70);
        GL.Vertex2(x + 20, y + 40);
        GL.End();
    }

    private void DrawPectoralFins(float x, float y, float rx)
    {
        GL.Begin(PrimitiveType.Triangles);
        GL.Color3(0.15f, 0.6f, 0.15f);
        
        GL.Vertex2(x , y + 20);
        GL.Vertex2(x + 20, y);
        GL.Vertex2(x, y - 20);
        
        GL.End();
    }
}