using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using Task2.Tools;

namespace Task2.Strategies.FishDrawingStrategies;

public class Fish1DrawingStrategy : FishDrawingStrategy
{
    public override void DrawBody(float x, float y, float rx, float ry)
    {
        GL.Begin(PrimitiveType.TriangleFan);
        GL.Color3(1.0f, 0.9f, 0.2f);
        GL.Vertex2(x, y);
        GL.Color3(1.0f, 0.7f, 0.0f);
        
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
        GL.Color3(1.0f, 0.7f, 0.0f);
        GL.Vertex2(x + rx, y);
        GL.Vertex2(x + rx + 70, y + 40);
        GL.Vertex2(x + rx + 90, y + 20);
        GL.Vertex2(x + rx + 90, y - 20);
        GL.Vertex2(x + rx + 70, y - 40);
        GL.Vertex2(x + rx, y);
        GL.End();
    }

    public override void DrawEye(float x, float y)
    {
        float eyeX = x - 45;
        float eyeY = y + 25;
        
        DrawingTools.DrawEllipse(eyeX, eyeY, 12, 14, Color4.White);
        DrawingTools.DrawEllipse(eyeX, eyeY, 8, 10, new Color4(0.3f, 0.6f, 1.0f, 1.0f));
        DrawingTools.DrawEllipse(eyeX, eyeY, 5, 7, Color4.Black);
        
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
}