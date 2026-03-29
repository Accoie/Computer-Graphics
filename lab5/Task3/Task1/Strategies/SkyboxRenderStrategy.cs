using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Task1.Strategies;

public class SkyboxRenderStrategy : IRenderStrategy
{
    private readonly int _skyboxTextureId;

    public SkyboxRenderStrategy(int skyboxTextureId)
    {
        _skyboxTextureId = skyboxTextureId;
    }

    public void Render(int[,] labyrinthMap, int mapSize)
    {
        GL.PushAttrib(AttribMask.AllAttribBits);

        GL.Disable(EnableCap.Lighting);
        GL.Disable(EnableCap.DepthTest);
        GL.DepthMask(false);

        GL.MatrixMode(MatrixMode.Modelview);
        GL.PushMatrix();

        Matrix4 viewMatrix = Matrix4.Identity; 

        GL.GetFloat(GetPName.ModelviewMatrix, out viewMatrix.Row0.X);

        viewMatrix.M41 = 0f; 
        viewMatrix.M42 = 0f;
        viewMatrix.M43 = 0f;
        viewMatrix.M44 = 1f;

        GL.LoadMatrix(ref viewMatrix);

        float size = 1000f;
        float half = size * 0.5f;

        GL.Enable(EnableCap.Texture2D);
        GL.BindTexture(TextureTarget.Texture2D, _skyboxTextureId);

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

        GL.Begin(PrimitiveType.Quads);

        GL.TexCoord2(0, 0);
        GL.Vertex3(-half, -half, half);
        GL.TexCoord2(1, 0);
        GL.Vertex3(half, -half, half);
        GL.TexCoord2(1, 1);
        GL.Vertex3(half, half, half);
        GL.TexCoord2(0, 1);
        GL.Vertex3(-half, half, half);

        GL.TexCoord2(1, 0);
        GL.Vertex3(-half, -half, -half);
        GL.TexCoord2(1, 1);
        GL.Vertex3(-half, half, -half);
        GL.TexCoord2(0, 1);
        GL.Vertex3(half, half, -half);
        GL.TexCoord2(0, 0);
        GL.Vertex3(half, -half, -half);

        GL.TexCoord2(0, 1);
        GL.Vertex3(-half, half, -half);
        GL.TexCoord2(0, 0);
        GL.Vertex3(-half, half, half);
        GL.TexCoord2(1, 0);
        GL.Vertex3(half, half, half);
        GL.TexCoord2(1, 1);
        GL.Vertex3(half, half, -half);

        GL.TexCoord2(0, 0);
        GL.Vertex3(-half, -half, -half);
        GL.TexCoord2(1, 0);
        GL.Vertex3(half, -half, -half);
        GL.TexCoord2(1, 1);
        GL.Vertex3(half, -half, half);
        GL.TexCoord2(0, 1);
        GL.Vertex3(-half, -half, half);

        GL.TexCoord2(0, 0);
        GL.Vertex3(half, -half, -half);
        GL.TexCoord2(0, 1);
        GL.Vertex3(half, half, -half);
        GL.TexCoord2(1, 1);
        GL.Vertex3(half, half, half);
        GL.TexCoord2(1, 0);
        GL.Vertex3(half, -half, half);

        // Левая (-X)
        GL.TexCoord2(1, 0);
        GL.Vertex3(-half, -half, -half);
        GL.TexCoord2(0, 0);
        GL.Vertex3(-half, -half, half);
        GL.TexCoord2(0, 1);
        GL.Vertex3(-half, half, half);
        GL.TexCoord2(1, 1);
        GL.Vertex3(-half, half, -half);

        GL.End();

        GL.PopMatrix();
        GL.DepthMask(true);
        GL.Enable(EnableCap.DepthTest);
        GL.PopAttrib();
    }

    public void Dispose()
    {
    }
}