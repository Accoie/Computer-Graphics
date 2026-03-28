using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Task3.Strategies;

public class WallRenderStrategy : IRenderStrategy
{
    private readonly float _wallHeight;
    
    private readonly Vector3 _wallColorSides = new(0.1f, 0.2f, 0.4f);
    private readonly Vector3 _wallColorTop = new(1f, 1f, 1f);
    
    public WallRenderStrategy(float wallHeight)
    {
        _wallHeight = wallHeight;
    }
    
    public void Render(int[,] map, int mapSize)
    {
        for (int z = 0; z < mapSize; z++)
        {
            for (int x = 0; x < mapSize; x++)
            {
                if (map[z, x] == 1)
                {
                    RenderWall(x, 0, z, _wallHeight);
                }
            }
        }
    }

    private void RenderWall(float x, float y, float z, float wallHeight)
    {
        float x0 = x;
        float x1 = x + 1;
        float z0 = z;
        float z1 = z + 1;

        GL.Begin(PrimitiveType.Quads);
        
        GL.Color3(_wallColorSides);
        
        RenderFrontFace(x0, x1, z1, y, wallHeight);
        RenderBackFace(x0, x1, z0, y, wallHeight);
        RenderLeftFace(x0, z0, z1, y, wallHeight);
        RenderRightFace(x1, z0, z1, y, wallHeight);
        
        RenderTopFace(x0, x1, z0, z1, y, wallHeight);
        
        GL.End();
    }

    private void RenderFrontFace(float x0, float x1, float z1, float y, float wallHeight)
    {
        GL.Normal3(0, 0, 1);
        GL.Vertex3(x0, y, z1);
        GL.Vertex3(x1, y, z1);
        GL.Vertex3(x1, y + wallHeight, z1);
        GL.Vertex3(x0, y + wallHeight, z1);
    }

    private void RenderBackFace(float x0, float x1, float z0, float y, float wallHeight)
    {
        GL.Normal3(0, 0, -1);
        GL.Vertex3(x0, y, z0);
        GL.Vertex3(x1, y, z0);
        GL.Vertex3(x1, y + wallHeight, z0);
        GL.Vertex3(x0, y + wallHeight, z0);
    }

    private void RenderLeftFace(float x0, float z0, float z1, float y, float wallHeight)
    {
        GL.Normal3(-1, 0, 0);
        GL.Vertex3(x0, y, z0);
        GL.Vertex3(x0, y, z1);
        GL.Vertex3(x0, y + wallHeight, z1);
        GL.Vertex3(x0, y + wallHeight, z0);
    }

    private void RenderRightFace(float x1, float z0, float z1, float y, float wallHeight)
    {
        GL.Normal3(1, 0, 0);
        GL.Vertex3(x1, y, z0);
        GL.Vertex3(x1, y, z1);
        GL.Vertex3(x1, y + wallHeight, z1);
        GL.Vertex3(x1, y + wallHeight, z0);
    }

    private void RenderTopFace(float x0, float x1, float z0, float z1, float y, float wallHeight)
    {
        GL.Color3(_wallColorTop);
        GL.Normal3(0, -1, 0);
        GL.Vertex3(x0, y + wallHeight, z0);
        GL.Vertex3(x1, y + wallHeight, z1);
        GL.Vertex3(x1, y + wallHeight, z0);
        GL.Vertex3(x0, y + wallHeight, z1);
    }
}
