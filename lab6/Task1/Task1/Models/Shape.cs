using Assimp;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Task1.Services;
using Task1.Shaders;
using PrimitiveType = OpenTK.Graphics.OpenGL4.PrimitiveType;

namespace Task1.Models;

public class Shape
{
    private Scene _scene = null!;
    private readonly Dictionary<Mesh, int> _meshVbos = [];
    private readonly Dictionary<Material, int> _textureIds = [];
    private readonly Dictionary<Mesh, int> _meshVertexCounts = [];

    private float _x;
    private float _y;
    private float _z;
    private readonly float _scale;

    private readonly bool _isInverse;

    private int _forcedTextureId = -1;
    private string _modelPath = null!;

    public float X
    {
        get => _x;
        set => _x = value;
    }

    public float Y
    {
        get => _y;
        set => _y = value;
    }

    public float Z
    {
        get => _z;
        set => _z = value;
    }

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

        _modelPath = path;

        string? modelDirectory = Path.GetDirectoryName(path);
        if (path.Contains("Board.3ds"))
        {
            foreach (Material material in _scene.Materials)
            {
                LoadMaterialTexture(material, modelDirectory!);
            }
        }

        foreach (Mesh mesh in _scene.Meshes)
        {
            ProcessMesh(mesh);
        }
    }

    public void Dispose()
    {
        foreach (int vbo in _meshVbos.Values)
        {
            GL.DeleteBuffer(vbo);
        }

        foreach (int textureId in _textureIds.Values)
        {
            TextureLoader.DeleteTexture(textureId);
        }
    }

    public void SetTexture(int textureId)
    {
        _forcedTextureId = textureId;
    }

    public void Paint(Shader shader)
    {
        Matrix4 model = Matrix4.CreateTranslation(_x, _y, _z);
        shader.SetMatrix4("model", model);

        foreach (Mesh mesh in _scene.Meshes)
        {
            int vbo = _meshVbos[mesh];
            int vertexCount = _meshVertexCounts[mesh];

            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            ConfigureVertexAttributes();

            Material material = _scene.Materials[mesh.MaterialIndex];
            SetupTextureAndColor(shader, material);

            GL.DrawArrays(PrimitiveType.Triangles, 0, vertexCount);
        }
    }

    private void ProcessMesh(Mesh mesh)
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

        List<float> vertexData = BuildVertexData(vertices, normals, texCoords, indices);

        int vbo = CreateBuffer(vertexData);
        _meshVbos[mesh] = vbo;
        _meshVertexCounts[mesh] = vertexData.Count / 8;
    }

    private List<float> BuildVertexData(Vector3[] vertices, Vector3[] normals, Vector2[] texCoords, int[] indices)
    {
        List<float> vertexData = new();

        for (int i = 0; i < indices.Length; i++)
        {
            int index = indices[i];

            vertexData.Add(vertices[index].X * _scale);
            vertexData.Add(vertices[index].Y * _scale);
            vertexData.Add(vertices[index].Z * _scale);

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

        return vertexData;
    }

    private int CreateBuffer(List<float> vertexData)
    {
        int vbo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, vertexData.Count * sizeof(float),
            vertexData.ToArray(), BufferUsageHint.DynamicDraw);
        return vbo;
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

    private void SetupTextureAndColor(Shader shader, Material material)
    {
        bool applyForced = _forcedTextureId != -1 && !_modelPath.Contains("Board.3ds");

        if (applyForced)
        {
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, _forcedTextureId);
            shader.SetInt("mainTexture", 0);
            shader.SetBool("useTexture", true);
        }
        else if (_textureIds.ContainsKey(material) && _textureIds[material] != 0)
        {
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, _textureIds[material]);
            shader.SetInt("mainTexture", 0);
            shader.SetBool("useTexture", true);
        }
        else
        {
            shader.SetBool("useTexture", false);
        }

        SetColorToShader(shader, material.ColorDiffuse, "diffuseColor");
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
                int textureId = TextureLoader.LoadTexture(fullPath);
                _textureIds[material] = textureId;
            }
        }
    }
}