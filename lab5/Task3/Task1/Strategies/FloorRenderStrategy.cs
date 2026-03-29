using OpenTK.Graphics.OpenGL;

namespace Task1.Strategies;

public class FloorRenderStrategy : IRenderStrategy
{
    private readonly int _textureId;

    public FloorRenderStrategy(int textureId)
    {
        _textureId = textureId;
    }

    public void Render(int[,] map, int mapSize)
    {
        GL.BindTexture(TextureTarget.Texture2D, _textureId);

        GL.Begin(PrimitiveType.Quads);
        GL.Normal3(0, 1, 0);

        for (int z = 0; z < mapSize; z++)
        {
            for (int x = 0; x < mapSize; x++)
            {
                RenderQuad(x, z);
            }
        }

        GL.End();

        GL.BindTexture(TextureTarget.Texture2D, 0);
    }

    private void RenderQuad(float x, float z)
    {
        GL.TexCoord2(0.0f, 0.0f);
        GL.Vertex3(x, 0, z);
        GL.TexCoord2(1.0f, 0.0f);
        GL.Vertex3(x + 1, 0, z);
        GL.TexCoord2(1.0f, 1.0f);
        GL.Vertex3(x + 1, 0, z + 1);
        GL.TexCoord2(0.0f, 1.0f);
        GL.Vertex3(x, 0, z + 1);
    }
}