using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using Task2.Components;

namespace Task2.Strategies.FishDrawingStrategies;

public class Fish5DrawingStrategy : IDrawingStrategy<Fish>
{
    public void Draw(Fish fish)
    {
        float x = fish.Position.X;
        float y = fish.Position.Y;
        float rx = fish.Size.X;
        float ry = fish.Size.Y;

        GL.PushMatrix();
        GL.Translate(-fish.Offset, 0.0f, 0.0f);

        DrawEllipse(x, y, rx, ry, new Color4(0.97f, 0.08f, 0.78f, 1.0f));

        GL.Begin(PrimitiveType.Polygon);
        GL.Color3(0.97f, 0.08f, 0.78f);
        GL.Vertex2(x + rx, y);
        GL.Color3(0.0f, 0.0f, 1f);
        GL.Vertex2(x + rx + 50, y + 50);
        GL.Color3(0.0f, 0.0f, 1f);
        GL.Vertex2(x + rx + 100.0f, y + 20.0f);
        GL.Vertex2(x + rx + 70.0f, y);
        GL.Vertex2(x + rx + 100.0f, y - 20.0f);
        GL.Color3(0.0f, 0.0f, 1f);
        GL.Vertex2(x + rx + 50, y - 50);
        GL.Color3(0.0f, 0.0f, 1f);
        GL.Vertex2(x + rx, y);
        GL.End();

        DrawEllipse(x - 50, y + 20.0f, ry / 2, ry / 2, Color4.White);
        DrawEllipse(x - 40.0f, y + 30.0f, ry / 4, ry / 4, Color4.Black);

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