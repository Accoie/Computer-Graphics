using OpenTK.Graphics.OpenGL;
using Task2.Components;

namespace Task2.Strategies.StoneRenderStrategies;

public class Stone3RenderStrategy : IRenderStrategy<Stone>
{
    public void Render(Stone stone)
    {
        float x = stone.Position.X;
        float y = stone.Position.Y;
        float width = stone.Size.X;
        float height = stone.Size.Y;

        GL.Begin(PrimitiveType.Polygon);

        GL.Color3(0.4f, 0.3f, 0.2f);
        GL.Vertex2(x - width / 2, y - height / 4);

        GL.Color3(0.5f, 0.4f, 0.3f);
        GL.Vertex2(x - width / 3, y + height / 3);

        GL.Color3(0.6f, 0.5f, 0.4f);
        GL.Vertex2(x + width / 4, y + height / 2);

        GL.Color3(0.5f, 0.4f, 0.3f);
        GL.Vertex2(x + width / 2, y);

        GL.Color3(0.4f, 0.3f, 0.2f);
        GL.Vertex2(x + width / 3, y - height / 2);
        GL.End();
    }
}