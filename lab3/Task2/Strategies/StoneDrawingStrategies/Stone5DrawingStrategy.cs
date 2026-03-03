using OpenTK.Graphics.OpenGL;
using Task2.Models;

namespace Task2.Strategies.StoneDrawingStrategies;

public class Stone5DrawingStrategy : IDrawingStrategy<Stone>
{
    private const int CountSegments = 8;
    public void Draw(Stone stone)
    {
        float centerX = stone.Position.X;
        float centerY = stone.Position.Y;
        float width = stone.Size.X;
        float height = stone.Size.Y;

        GL.Begin(PrimitiveType.TriangleFan);
        GL.Color3(0.4f, 0.3f, 0.2f);
        GL.Vertex2(centerX, centerY);

        for (int i = 0; i <= CountSegments; i++)
        {
            float angle = i * 2 * (float)Math.PI / CountSegments;
            GL.Color3(0.5f, 0.4f, 0.3f);
            GL.Vertex2(
                centerX + width * (float)Math.Cos(angle),
                centerY + height * (float)Math.Sin(angle)
            );
        }
        GL.End();
    }
}