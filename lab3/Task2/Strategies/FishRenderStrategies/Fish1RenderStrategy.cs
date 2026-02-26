using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using Task2.Components;

namespace Task2.Strategies.FishRenderStrategies;

public class Fish1RenderStrategy : IRenderStrategy<Fish>
{
    public void Render(Fish fish)
    {
        float x = fish.Position.X;
        float y = fish.Position.Y;
        float rx = fish.Size.X;
        float ry = fish.Size.Y;

        GL.PushMatrix();
        GL.Translate(-fish.Offset, 0.0f, 0.0f);

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

        GL.Begin(PrimitiveType.TriangleFan);
        GL.Color3(1.0f, 0.7f, 0.0f);
        GL.Vertex2(x + rx, y);
        GL.Vertex2(x + rx + 70, y + 40);
        GL.Vertex2(x + rx + 90, y + 20);
        GL.Vertex2(x + rx + 90, y - 20);
        GL.Vertex2(x + rx + 70, y - 40);
        GL.Vertex2(x + rx, y);
        GL.End();

        DrawEllipse(x - 45, y + 25, 12, 14, Color4.White);
        DrawEllipse(x - 45, y + 25, 8, 10, new Color4(0.3f, 0.6f, 1.0f, 1.0f));
        DrawEllipse(x - 45, y + 25, 5, 7, Color4.Black);

        GL.Begin(PrimitiveType.Points);
        GL.PointSize(3.0f);
        GL.Color3(1.0f, 1.0f, 1.0f);
        GL.Vertex2(x - 42, y + 29);
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