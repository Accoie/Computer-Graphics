using OpenTK.Graphics.OpenGL;
using Task2.Models;

namespace Task2.Strategies.StoneDrawingStrategies;

public class Stone2DrawingStrategy : IDrawingStrategy<Stone>
{
    public void Draw(Stone stone)
    {
        float x = stone.Position.X;
        float y = stone.Position.Y;
        float width = stone.Size.X;
        float height = stone.Size.Y;

        GL.Begin(PrimitiveType.TriangleFan);
        GL.Color3(0.5f, 0.5f, 0.5f); 
        GL.Vertex2(x, y);

        for (int i = 0; i <= 10; i++)
        {
            float angle = i * 2 * (float)Math.PI / 10;
            GL.Vertex2(
                x + width * (float)Math.Cos(angle),
                y + height * (float)Math.Sin(angle)
            );
        }
        GL.End();
    }
}