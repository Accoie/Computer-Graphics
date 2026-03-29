using OpenTK.Graphics.OpenGL;

namespace Task1.Strategies;

public class WallRenderStrategy : IRenderStrategy
{
    private const float RepetitionRatio = 0.1f;
    private readonly int _shadowMapTextureId;
    private readonly float _wallHeight;
    private readonly int[] _wallTextures;

    public WallRenderStrategy(float wallHeight, int[] wallTextures, int shadowMapTextureId)
    {
        _wallHeight = wallHeight;
        _wallTextures = wallTextures;
        _shadowMapTextureId = shadowMapTextureId;
    }

    public void Render(int[,] map, int mapSize)
    {
        SetupAoTexture();

        for (int z = 0; z < mapSize; z++)
        {
            for (int x = 0; x < mapSize; x++)
            {
                if (map[z, x] == 1)
                {
                    RenderWall(x, 0f, z, _wallHeight);
                }
            }
        }

        ResetTextureState();
    }

    private void SetupAoTexture()
    {
        GL.ActiveTexture(TextureUnit.Texture1);
        GL.BindTexture(TextureTarget.Texture2D, _shadowMapTextureId);

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
    }

    private void RenderWall(float x, float y, float z, float wallHeight)
    {
        float x0 = x;
        float x1 = x + 1f;
        float z0 = z;
        float z1 = z + 1f;

        int textureIndex = ((int)x + (int)z) % _wallTextures.Length;

        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2D, _wallTextures[textureIndex]);

        GL.ActiveTexture(TextureUnit.Texture1);
        GL.BindTexture(TextureTarget.Texture2D, _shadowMapTextureId);

        GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (int)TextureEnvMode.Modulate);

        GL.Color3(1f, 1f, 1f);

        GL.Begin(PrimitiveType.Quads);

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

        GL.MultiTexCoord2(TextureUnit.Texture0, 0f, 0f);
        GL.MultiTexCoord2(TextureUnit.Texture1, x0 * RepetitionRatio, y * RepetitionRatio);
        GL.Vertex3(x0, y, z1);

        GL.MultiTexCoord2(TextureUnit.Texture0, 1f, 0f);
        GL.MultiTexCoord2(TextureUnit.Texture1, x1 * RepetitionRatio, y * RepetitionRatio);
        GL.Vertex3(x1, y, z1);

        GL.MultiTexCoord2(TextureUnit.Texture0, 1f, 1f);
        GL.MultiTexCoord2(TextureUnit.Texture1, x1 * RepetitionRatio, (y + wallHeight) * RepetitionRatio);
        GL.Vertex3(x1, y + wallHeight, z1);

        GL.MultiTexCoord2(TextureUnit.Texture0, 0f, 1f);
        GL.MultiTexCoord2(TextureUnit.Texture1, x0 * RepetitionRatio, (y + wallHeight) * RepetitionRatio);
        GL.Vertex3(x0, y + wallHeight, z1);
    }

    private void RenderBackFace(float x0, float x1, float z0, float y, float wallHeight)
    {
        GL.Normal3(0, 0, -1);

        GL.MultiTexCoord2(TextureUnit.Texture0, 0f, 0f);
        GL.MultiTexCoord2(TextureUnit.Texture1, x0 * RepetitionRatio, y * RepetitionRatio);
        GL.Vertex3(x0, y, z0);

        GL.MultiTexCoord2(TextureUnit.Texture0, 1f, 0f);
        GL.MultiTexCoord2(TextureUnit.Texture1, x1 * RepetitionRatio, y * RepetitionRatio);
        GL.Vertex3(x1, y, z0);

        GL.MultiTexCoord2(TextureUnit.Texture0, 1f, 1f);
        GL.MultiTexCoord2(TextureUnit.Texture1, x1 * RepetitionRatio, (y + wallHeight) * RepetitionRatio);
        GL.Vertex3(x1, y + wallHeight, z0);

        GL.MultiTexCoord2(TextureUnit.Texture0, 0f, 1f);
        GL.MultiTexCoord2(TextureUnit.Texture1, x0 * RepetitionRatio, (y + wallHeight) * RepetitionRatio);
        GL.Vertex3(x0, y + wallHeight, z0);
    }

    private void RenderLeftFace(float x0, float z0, float z1, float y, float wallHeight)
    {
        GL.Normal3(-1, 0, 0);

        GL.MultiTexCoord2(TextureUnit.Texture0, 0f, 0f);
        GL.MultiTexCoord2(TextureUnit.Texture1, z0 * RepetitionRatio, y * RepetitionRatio);
        GL.Vertex3(x0, y, z0);

        GL.MultiTexCoord2(TextureUnit.Texture0, 1f, 0f);
        GL.MultiTexCoord2(TextureUnit.Texture1, z1 * RepetitionRatio, y * RepetitionRatio);
        GL.Vertex3(x0, y, z1);

        GL.MultiTexCoord2(TextureUnit.Texture0, 1f, 1f);
        GL.MultiTexCoord2(TextureUnit.Texture1, z1 * RepetitionRatio, (y + wallHeight) * RepetitionRatio);
        GL.Vertex3(x0, y + wallHeight, z1);

        GL.MultiTexCoord2(TextureUnit.Texture0, 0f, 1f);
        GL.MultiTexCoord2(TextureUnit.Texture1, z0 * RepetitionRatio, (y + wallHeight) * RepetitionRatio);
        GL.Vertex3(x0, y + wallHeight, z0);
    }

    private void RenderRightFace(float x1, float z0, float z1, float y, float wallHeight)
    {
        GL.Normal3(1, 0, 0);

        GL.MultiTexCoord2(TextureUnit.Texture0, 0f, 0f);
        GL.MultiTexCoord2(TextureUnit.Texture1, z0 * RepetitionRatio, y * RepetitionRatio);
        GL.Vertex3(x1, y, z0);

        GL.MultiTexCoord2(TextureUnit.Texture0, 1f, 0f);
        GL.MultiTexCoord2(TextureUnit.Texture1, z1 * RepetitionRatio, y * RepetitionRatio);
        GL.Vertex3(x1, y, z1);

        GL.MultiTexCoord2(TextureUnit.Texture0, 1f, 1f);
        GL.MultiTexCoord2(TextureUnit.Texture1, z1 * RepetitionRatio, (y + wallHeight) * RepetitionRatio);
        GL.Vertex3(x1, y + wallHeight, z1);

        GL.MultiTexCoord2(TextureUnit.Texture0, 0f, 1f);
        GL.MultiTexCoord2(TextureUnit.Texture1, z0 * RepetitionRatio, (y + wallHeight) * RepetitionRatio);
        GL.Vertex3(x1, y + wallHeight, z0);
    }

    private void RenderTopFace(float x0, float x1, float z0, float z1, float y, float wallHeight)
    {
        GL.Normal3(0, 1, 0);

        GL.MultiTexCoord2(TextureUnit.Texture0, 0f, 0f);
        GL.MultiTexCoord2(TextureUnit.Texture1, x0 * RepetitionRatio, z0 * RepetitionRatio);
        GL.Vertex3(x0, y + wallHeight, z0);

        GL.MultiTexCoord2(TextureUnit.Texture0, 1f, 0f);
        GL.MultiTexCoord2(TextureUnit.Texture1, x1 * RepetitionRatio, z0 * RepetitionRatio);
        GL.Vertex3(x1, y + wallHeight, z0);

        GL.MultiTexCoord2(TextureUnit.Texture0, 1f, 1f);
        GL.MultiTexCoord2(TextureUnit.Texture1, x1 * RepetitionRatio, z1 * RepetitionRatio);
        GL.Vertex3(x1, y + wallHeight, z1);

        GL.MultiTexCoord2(TextureUnit.Texture0, 0f, 1f);
        GL.MultiTexCoord2(TextureUnit.Texture1, x0 * RepetitionRatio, z1 * RepetitionRatio);
        GL.Vertex3(x0, y + wallHeight, z1);
    }

    private void ResetTextureState()
    {
        GL.ActiveTexture(TextureUnit.Texture1);
        GL.BindTexture(TextureTarget.Texture2D, 0);

        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2D, 0);

        GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (int)TextureEnvMode.Replace);

        GL.Color3(1f, 1f, 1f);
    }
}