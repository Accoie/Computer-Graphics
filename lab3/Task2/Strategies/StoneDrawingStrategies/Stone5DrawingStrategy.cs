using OpenTK.Graphics.OpenGL;
using Task2.Components;

namespace Task2.Strategies.StoneDrawingStrategies;

public class Stone5DrawingStrategy : IDrawingStrategy<Stone>
{
    public void Draw(Stone stone)
    {
        float x = stone.Position.X;
        float y = stone.Position.Y;
        float width = stone.Size.X;
        float height = stone.Size.Y;

        GL.Begin(PrimitiveType.TriangleFan);
        GL.Color3(0.4f, 0.3f, 0.2f);
        GL.Vertex2(x, y);

        for (int i = 0; i <= 8; i++)
        {
            float angle = i * 2 * (float)Math.PI / 8;
            GL.Color3(0.5f, 0.4f, 0.3f);
            GL.Vertex2(
                x + width * (float)Math.Cos(angle),
                y + height * (float)Math.Sin(angle)
            );
        }
        GL.End();
    }
}