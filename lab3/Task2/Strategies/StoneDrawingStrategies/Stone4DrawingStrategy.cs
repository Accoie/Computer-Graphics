using OpenTK.Graphics.OpenGL;
using Task2.Models;

namespace Task2.Strategies.StoneDrawingStrategies;

public class Stone4DrawingStrategy : IDrawingStrategy<Stone>
{
    public void Draw(Stone stone)
    {
        float x = stone.Position.X;
        float y = stone.Position.Y;
        float width = stone.Size.X;
        float height = stone.Size.Y;

        GL.Begin(PrimitiveType.TriangleFan);
        GL.Color3(0.7f, 0.6f, 0.5f);
        GL.Vertex2(x, y);

        for (int i = 0; i <= 8; i++)
        {
            float angle = i * 2 * (float)Math.PI / 8;
            float shade = 0.2f * (i % 3);
            GL.Color3(0.5f + shade, 0.7f + shade, 0.9f + shade);
            GL.Vertex2(
                x + width * (float)Math.Cos(angle),
                y + height * (float)Math.Sin(angle) * 0.3f
            );
        }
        GL.End();
    }
}