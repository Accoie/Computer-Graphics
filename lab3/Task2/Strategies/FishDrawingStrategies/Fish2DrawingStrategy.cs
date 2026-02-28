using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using Task2.Components;

namespace Task2.Strategies.FishDrawingStrategies;

public class Fish2DrawingStrategy : IDrawingStrategy<Fish>
{
    public void Draw(Fish fish)
    {
        float x = fish.Position.X;
        float y = fish.Position.Y;
        float rx = fish.Size.X;
        float ry = fish.Size.Y;

        GL.PushMatrix();
        GL.Translate(-fish.Offset, 0.0f, 0.0f);

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

        GL.Begin(PrimitiveType.Triangles);
        GL.Color3(0.9f, 0.0f, 0.0f);
        GL.Vertex2(x + rx, y);
        GL.Vertex2(x + rx + 60, y + 50);
        GL.Vertex2(x + rx + 40, y);

        GL.Vertex2(x + rx, y);
        GL.Vertex2(x + rx + 60, y - 50);
        GL.Vertex2(x + rx + 40, y);
        GL.End();

        DrawEllipse(x - 35, y + 12, 6, 8, Color4.White);
        DrawEllipse(x - 33, y + 14, 3, 4, Color4.Black);

        GL.Begin(PrimitiveType.Points);
        GL.PointSize(2.0f);
        GL.Color3(1.0f, 1.0f, 1.0f);
        GL.Vertex2(x - 31, y + 16);
        GL.End();

        GL.PopMatrix();
    }

    private void DrawEllipse(float x, float y, float radiusX, float radiusY, Color4 color, PrimitiveType primitiveType = PrimitiveType.Polygon)
    {
        GL.Begin(primitiveType);
        GL.Color4(color);

        int segments = 360;

        for (int i = 0; i < segments; i++)
        {
            float degInRad = i * (float)Math.PI / 180;
            GL.Vertex2(x + Math.Cos(degInRad) * radiusX, y + Math.Sin(degInRad) * radiusY);
        }
        GL.End();
    }
}