using OpenTK.Graphics.OpenGL;

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
        bool fogWasEnabled = GL.IsEnabled(EnableCap.Fog);
        if (fogWasEnabled)
        {
            GL.Disable(EnableCap.Fog);
        }

        SetupSkyboxTexture();
        RenderSkybox();
        
        if (fogWasEnabled)
        {
            GL.Enable(EnableCap.Fog);
        }
        
    }

    private void SetupSkyboxTexture()
    {
        GL.Enable(EnableCap.Texture2D);
        GL.BindTexture(TextureTarget.Texture2D, _skyboxTextureId);
        
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
    }

    private void RenderSkybox()
    {
        float size = 1000f;
        float half = size * 0.5f;
        
        GL.Begin(PrimitiveType.Quads);
        
        RenderFrontFace(half);
        RenderBackFace(half);
        RenderTopFace(half);
        RenderBottomFace(half);
        RenderRightFace(half);
        RenderLeftFace(half);
        
        GL.End();
    }

    private void RenderFrontFace(float half)
    {
        GL.TexCoord2(0, 0);
        GL.Vertex3(-half, -half, half);
        GL.TexCoord2(1, 0);
        GL.Vertex3(half, -half, half);
        GL.TexCoord2(1, 1);
        GL.Vertex3(half, half, half);
        GL.TexCoord2(0, 1);
        GL.Vertex3(-half, half, half);
    }

    private void RenderBackFace(float half)
    {
        GL.TexCoord2(1, 0);
        GL.Vertex3(-half, -half, -half);
        GL.TexCoord2(1, 1);
        GL.Vertex3(-half, half, -half);
        GL.TexCoord2(0, 1);
        GL.Vertex3(half, half, -half);
        GL.TexCoord2(0, 0);
        GL.Vertex3(half, -half, -half);
    }

    private void RenderTopFace(float half)
    {
        GL.TexCoord2(0, 1);
        GL.Vertex3(-half, half, -half);
        GL.TexCoord2(0, 0);
        GL.Vertex3(-half, half, half);
        GL.TexCoord2(1, 0);
        GL.Vertex3(half, half, half);
        GL.TexCoord2(1, 1);
        GL.Vertex3(half, half, -half);
    }

    private void RenderBottomFace(float half)
    {
        GL.TexCoord2(0, 0);
        GL.Vertex3(-half, -half, -half);
        GL.TexCoord2(1, 0);
        GL.Vertex3(half, -half, -half);
        GL.TexCoord2(1, 1);
        GL.Vertex3(half, -half, half);
        GL.TexCoord2(0, 1);
        GL.Vertex3(-half, -half, half);
    }

    private void RenderRightFace(float half)
    {
        GL.TexCoord2(0, 0);
        GL.Vertex3(half, -half, -half);
        GL.TexCoord2(0, 1);
        GL.Vertex3(half, half, -half);
        GL.TexCoord2(1, 1);
        GL.Vertex3(half, half, half);
        GL.TexCoord2(1, 0);
        GL.Vertex3(half, -half, half);
    }

    private void RenderLeftFace(float half)
    {
        GL.TexCoord2(1, 0);
        GL.Vertex3(-half, -half, -half);
        GL.TexCoord2(0, 0);
        GL.Vertex3(-half, -half, half);
        GL.TexCoord2(0, 1);
        GL.Vertex3(-half, half, half);
        GL.TexCoord2(1, 1);
        GL.Vertex3(-half, half, -half);
    }
}