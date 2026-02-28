using OpenTK.Graphics.OpenGL;
using Task2.Components;

namespace Task2.Strategies.StoneDrawingStrategies;

public class Stone1DrawingStrategy : IDrawingStrategy<Stone>
{
    public void Draw(Stone stone)
    {
        float x = stone.Position.X;
        float y = stone.Position.Y;
        float width = stone.Size.X;
        float height = stone.Size.Y;

        GL.Begin(PrimitiveType.TriangleFan);
        GL.Color3(0.3f, 0.2f, 0.1f);
        GL.Vertex2(x, y);

        for (int i = 0; i <= 12; i++)
        {
            float angle = i * 2 * (float)Math.PI / 12;
            float brightness = 0.5f + 0.3f * (float)Math.Sin(angle);
            GL.Color3(brightness * 0.6f, brightness * 0.4f, brightness * 0.2f);
            GL.Vertex2(
                x + width * (float)Math.Cos(angle) * 0.8f,
                y + height * (float)Math.Sin(angle) * 0.5f
            );
        }
        GL.End();
    }
}