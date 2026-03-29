using OpenTK.Graphics.OpenGL;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Task1.Services;

public class TextureService
{
    public static int LoadTexture(string filePath)
    {
        return LoadTextureInternal(
            filePath,
            TextureMinFilter.LinearMipmapLinear,
            TextureMagFilter.Linear,
            true);
    }

    public static int LoadShadowMap(string filePath)
    {
        return LoadTextureInternal(
            filePath,
            TextureMinFilter.Linear,
            TextureMagFilter.Linear,
            false);
    }

    public static void DeleteTexture(int textureId)
    {
        if (textureId != 0)
        {
            GL.DeleteTexture(textureId);
        }
    }

    private static int LoadTextureInternal(
        string filePath,
        TextureMinFilter minFilter,
        TextureMagFilter magFilter,
        bool generateMipmaps)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("Texture file not found: " + filePath);
        }

        using Image<Rgba32> image = Image.Load<Rgba32>(filePath);
        image.Mutate(x => x.Flip(FlipMode.Vertical));

        int textureId = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2D, textureId);

        byte[] pixelData = new byte[image.Width * image.Height * 4];
        image.CopyPixelDataTo(pixelData);

        GL.TexImage2D(
            TextureTarget.Texture2D,
            0,
            PixelInternalFormat.Rgba,
            image.Width,
            image.Height,
            0,
            PixelFormat.Rgba,
            PixelType.UnsignedByte,
            pixelData);

        SetupTextureSampling(minFilter, magFilter);

        if (generateMipmaps)
        {
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        }

        return textureId;
    }

    private static void SetupTextureSampling(TextureMinFilter minFilter, TextureMagFilter magFilter)
    {
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)minFilter);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)magFilter);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
    }
}