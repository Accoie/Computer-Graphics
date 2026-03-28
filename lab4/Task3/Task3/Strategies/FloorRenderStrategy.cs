using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Task3.Strategies;

public class FloorRenderStrategy : IRenderStrategy
{
    private static readonly Vector3 FloorColorPassage = new(0.45f, 0.05f, 0.18f);
    private static readonly Vector3 FloorColorWall = new(0.45f, 0.05f, 0.35f);
    
    public void Render(int[,] map, int mapSize)
    {
        GL.Begin(PrimitiveType.Quads);
        GL.Normal3(0, 1, 0);

        for (int z = 0; z < mapSize; z++)
        {
            for (int x = 0; x < mapSize; x++)
            {
                bool isWall = map[z, x] == 1;
                GL.Color3(isWall ? FloorColorWall : FloorColorPassage);
                RenderQuad(x, 0, z);
            }
        }

        GL.End();
    }

    private void RenderQuad(float x, float y, float z)
    {
        GL.Vertex3(x, y, z);
        GL.Vertex3(x + 1, y, z);
        GL.Vertex3(x + 1, y, z + 1);
        GL.Vertex3(x, y, z + 1);
    }
}