using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using Task3.Shaders;

namespace Task3.Shapes;

public class Torus
{
    private readonly TorusData _data;
    private readonly Vector3 _color;
    private readonly int _slices;
    private readonly int _stacks;

    private BufferData _bufferData;

    public TorusData Data => _data;

    public Torus(TorusData data, Vector3 color, int slices = 50, int stacks = 30)
    {
        _data = data;
        _color = color;
        _slices = slices;
        _stacks = stacks;
    }

    public void CreateBuffers()
    {
        float[] points = CreateTorusPoints(_data, _color, _slices, _stacks);
        _bufferData = CreateBufferData(points);
    }

    public void Paint(Shader shader)
    {
        shader.SetMatrix4("model", _data.ModelMatrix);

        GL.BindVertexArray(_bufferData.Vao);
        GL.DrawArrays(PrimitiveType.TriangleStrip, 0, _bufferData.VertexCount);
        GL.BindVertexArray(0);
    }

    private static float[] CreateTorusPoints(TorusData torus, Vector3 color, int slices, int stacks)
    {
        List<float> data = new List<float>();

        for (int i = 0; i < slices; i++)
        {
            float u0 = (float)i / slices * 2 * MathF.PI;
            float u1 = (float)(i + 1) / slices * 2 * MathF.PI;

            for (int j = 0; j <= stacks; j++)
            {
                float v = (float)j / stacks * 2 * MathF.PI;

                AddTorusVertex(data, torus, u0, v, color);
                AddTorusVertex(data, torus, u1, v, color);
            }
        }

        return data.ToArray();
    }

    private static void AddTorusVertex(List<float> data, TorusData torus, float u, float v, Vector3 color)
    {
        float majorRadius = torus.MajorRadius;
        float minorRadius = torus.MinorRadius;

        float cosU = MathF.Cos(u);
        float sinU = MathF.Sin(u);
        float cosV = MathF.Cos(v);
        float sinV = MathF.Sin(v);

        float x = (majorRadius + minorRadius * cosV) * cosU;
        float y = (majorRadius + minorRadius * cosV) * sinU;
        float z = minorRadius * sinV;

        Vector3 pos = new(x, y, z);

        Vector3 normal = new(cosV * cosU, cosV * sinU, sinV);
        normal = Vector3.Normalize(normal);

        data.AddRange([
            pos.X, pos.Z, pos.Y,
            normal.X, normal.Z, normal.Y,
            color.X, color.Y, color.Z
        ]);
    }

    private BufferData CreateBufferData(float[] points)
    {
        int vao = GL.GenVertexArray();
        GL.BindVertexArray(vao);

        int vbo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, points.Length * sizeof(float), points, BufferUsageHint.StaticDraw);
        ConfigureShaderLayout();

        GL.BindVertexArray(0);

        return new BufferData(vao, points.Length / 9);
    }

    private static void ConfigureShaderLayout()
    {
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 9 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 9 * sizeof(float), 3 * sizeof(float));
        GL.EnableVertexAttribArray(1);

        GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, 9 * sizeof(float), 6 * sizeof(float));
        GL.EnableVertexAttribArray(2);
    }
}
