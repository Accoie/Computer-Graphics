using OpenTK.Graphics.OpenGL;
using Task2.Models;

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
        GL.Color3(0.8f, 0.8f, 0.5f); 

        GL.Vertex2(x, y);

        for (int i = 0; i <= 12; i++)
        {
            float angle = i * 2 * (float)Math.PI / 12;
            GL.Vertex2(
                x + width * (float)Math.Cos(angle) * 0.8f,
                y + height * (float)Math.Sin(angle) * 0.5f
            );
        }
        GL.End();
    }
}