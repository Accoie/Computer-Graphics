using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using Task2.Tools;

namespace Task2.Strategies.FishDrawingStrategies;

public class Fish3DrawingStrategy : FishDrawingStrategy
{
    public override void DrawBody(float x, float y, float rx, float ry)
    {
        GL.Begin(PrimitiveType.TriangleFan);
        GL.Color3(0.3f, 0.5f, 1.0f);
        GL.Vertex2(x, y);
        GL.Color3(0.1f, 0.3f, 0.9f);
        
        for (int i = 0; i <= 360; i += 30)
        {
            float angle = i * (float)Math.PI / 180;
            GL.Vertex2(x + rx * (float)Math.Cos(angle), y + ry * (float)Math.Sin(angle));
        }
        GL.End();
    }

    public override void DrawTail(float x, float y, float rx)
    {
        GL.Begin(PrimitiveType.TriangleFan);
        GL.Color3(0.1f, 0.3f, 0.8f);
        GL.Vertex2(x + rx, y);
        GL.Vertex2(x + rx + 80, y + 30);
        GL.Vertex2(x + rx + 100, y);
        GL.Vertex2(x + rx + 80, y - 30);
        GL.Vertex2(x + rx, y);
        GL.End();
    }

    public override void DrawEye(float x, float y)
    {
        float eyeX = x - 20;
        float eyeY = y + 20;
        
        DrawingTools.DrawEllipse(eyeX, eyeY, 8, 10, Color4.White);
        DrawingTools.DrawEllipse(eyeX, eyeY, 5, 7, new Color4(0.3f, 0.9f, 0.5f, 1.0f));
        DrawingTools.DrawEllipse(eyeX, eyeY, 3, 4, Color4.Black);
        
        DrawEyeHighlight(eyeX, eyeY);
    }

    private void DrawEyeHighlight(float eyeX, float eyeY)
    {
        GL.Begin(PrimitiveType.Points);
        GL.PointSize(2.0f);
        GL.Color3(1.0f, 1.0f, 1.0f);
        GL.Vertex2(eyeX + 2, eyeY + 3);
        GL.End();
    }
}