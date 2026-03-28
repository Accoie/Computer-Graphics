using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Task3.Strategies;

public class CeilingRenderStrategy : IRenderStrategy
{
    private static readonly Vector3 CeilingColor = new(0.3f, 0.3f, 0.5f);
    private readonly float _wallHeight;

    public CeilingRenderStrategy(float wallHeight)
    {
        _wallHeight = wallHeight;
    }

    public void Render(int[,] map, int mapSize)
    {
        GL.Begin(PrimitiveType.Quads);
        GL.Color3(CeilingColor);

        for (int z = 0; z < mapSize; z++)
        {
            for (int x = 0; x < mapSize; x++)
            {
                if (map[z, x] == 1)
                {
                    continue;
                }

                RenderQuad(x, z);
            }
        }

        GL.End();
    }

    private void RenderQuad(float x, float z)
    {
        GL.Normal3(0, -1, 0);

        GL.Vertex3(x, _wallHeight, z);
        GL.Vertex3(x + 1, _wallHeight, z);
        GL.Vertex3(x + 1, _wallHeight, z + 1);
        GL.Vertex3(x, _wallHeight, z + 1);
    }
}