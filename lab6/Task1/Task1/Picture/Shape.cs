using Assimp;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using Task1.Shaders;
using Task1.TextureService;
using PrimitiveType = OpenTK.Graphics.OpenGL4.PrimitiveType;

namespace Task1.Picture;

public class Shape
{
    Scene _scene;
    Dictionary<Mesh, (int vbo, int ebo)> _meshBuffers = [];
    Dictionary<Material, int> _textureIds = [];
    Dictionary<Mesh, bool> _hasTexture = [];

    float _x;
    float _y;
    float _z;
    float _scale;

    bool _isInverse;

    public Shape(
        float x,
        float y,
        float z,
        float scale,
        bool isInverse = false)
    {
        _x = x;
        _y = y;
        _z = z;
        _scale = scale;
        _isInverse = isInverse;
    }

    public void LoadPicture(string path)
    {
        AssimpContext context = new AssimpContext();
        _scene = context.ImportFile(path,
            PostProcessSteps.Triangulate |
            PostProcessSteps.GenerateNormals |
            PostProcessSteps.CalculateTangentSpace |
            PostProcessSteps.FlipUVs
        );

        string? modelDirectory = Path.GetDirectoryName(path);
        foreach (Material material in _scene.Materials)
        {
            LoadMaterialTexture(material, modelDirectory);
        }

        foreach (Mesh mesh in _scene.Meshes)
        {
            Vector3[] vertices = mesh.Vertices
                .Select(v => new Vector3(v.X, v.Y, v.Z))
                .ToArray();

            Vector3[] normals = mesh.Normals
                .Select(n => new Vector3(n.X, n.Y, n.Z))
                .ToArray();

            Vector2[] texCoords = mesh.TextureCoordinateChannels[0]?
                .Select(t => new Vector2(t.X, t.Y))
                .ToArray() ?? Array.Empty<Vector2>();

            int[] indices = mesh.GetIndices();

            List<float> vertexData = new();

            for (int i = 0; i < indices.Length; i++)
            {
                int index = indices[i];

                vertexData.Add(_x + vertices[index].X * _scale);
                vertexData.Add(_y + vertices[index].Y * _scale);
                vertexData.Add(_z + vertices[index].Z * _scale);
                
                vertexData.Add(normals[index].X);
                vertexData.Add(normals[index].Y);
                vertexData.Add(normals[index].Z);
                
                if (texCoords.Length > index)
                {
                    vertexData.Add(texCoords[index].X);
                    vertexData.Add(texCoords[index].Y);
                }
                else
                {
                    vertexData.Add(0f);
                    vertexData.Add(0f);
                }
            }

            int vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertexData.Count * sizeof(float), vertexData.ToArray(), BufferUsageHint.StaticDraw);

            int ebo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(int), indices, BufferUsageHint.StaticDraw);

            _meshBuffers[mesh] = (vbo, ebo);
            
            Material mat = _scene.Materials[mesh.MaterialIndex];
            _hasTexture[mesh] = _textureIds.ContainsKey(mat) && _textureIds[mat] != 0;
        }
    }

    private void LoadMaterialTexture(Material material, string modelDirectory)
    {
        if (material.HasTextureDiffuse)
        {
            TextureSlot textureSlot = material.TextureDiffuse;
            string texturePath = textureSlot.FilePath;
            
            string fullPath = Path.Combine(modelDirectory, texturePath);
            if (!File.Exists(fullPath))
            {
                fullPath = Path.Combine(modelDirectory, Path.GetFileName(texturePath));
            }
            if (!File.Exists(fullPath))
            {
                fullPath = texturePath;
            }
            
            if (File.Exists(fullPath))
            {
                try
                {
                    int textureId = TextureLoader.LoadTexture(fullPath);
                    _textureIds[material] = textureId;
                    Console.WriteLine($"Texture loaded: {fullPath}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to load texture: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine($"Texture file not found: {fullPath}");
            }
        }
    }

    public void Paint(Shader shader)
    {
        foreach (Mesh mesh in _scene.Meshes)
        {
            (int vbo, int ebo) = _meshBuffers[mesh];

            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);

            ConfigureVertexAttributes();

            Material material = _scene.Materials[mesh.MaterialIndex];
            
            if (_hasTexture[mesh] && _textureIds.ContainsKey(material))
            {
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, _textureIds[material]);
                shader.SetInt("mainTexture", 0);
                shader.SetBool("useTexture", true);
            }
            else
            {
                shader.SetBool("useTexture", false);
                SetColorToShader(shader, material.ColorAmbient, "ambientColor");
                SetColorToShader(shader, material.ColorDiffuse, "diffuseColor");
            }

            GL.DrawElements(PrimitiveType.Triangles, mesh.GetIndices().Length, DrawElementsType.UnsignedInt, IntPtr.Zero);
        }
    }

    private void ConfigureVertexAttributes()
    {
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));
        GL.EnableVertexAttribArray(1);

        GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 6 * sizeof(float));
        GL.EnableVertexAttribArray(2);
    }
    
    private void SetColorToShader(Shader shader, Color4D color, string name)
    {
        float r = _isInverse ? 1 - color.R : color.R;
        float g = _isInverse ? 1 - color.G : color.G;
        float b = _isInverse ? 1 - color.B : color.B;

        shader.SetVector3(name, new Vector3(r, g, b));
    }

    public void Dispose()
    {
        foreach (var buffer in _meshBuffers.Values)
        {
            GL.DeleteBuffer(buffer.vbo);
            GL.DeleteBuffer(buffer.ebo);
        }
        
        foreach (var textureId in _textureIds.Values)
        {
            TextureLoader.DeleteTexture(textureId);
        }
    }
}

