using OpenTK.Graphics.OpenGL;

namespace Task2.Figures;

public abstract class Figure
{
    List<BufferData> _buffers = [];

    public abstract void CreatePoints();

    public void Paint()
    {
        foreach (BufferData buffer in _buffers)
        {
            GL.BindVertexArray(buffer.Vao);
            GL.DrawArrays(buffer.PrimitiveType, 0, buffer.VertexCount);
        }
    }

    protected void CreateAndSetUpBuffer(float[] points, PrimitiveType primitiveType)
    {
        int vao = GL.GenVertexArray();
        GL.BindVertexArray(vao);

        int vbo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, points.Length * sizeof(float), points, BufferUsageHint.StaticDraw);
    
        ConfigurateShaders(6); 

        GL.BindVertexArray(0);

        _buffers.Add(new BufferData(vao, primitiveType, points.Length / 6));
    }

    private void ConfigurateShaders(int strideCount)
    {
        int stride = strideCount * sizeof(float);

        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, stride, 0);
        GL.EnableVertexAttribArray(0);

        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, stride, 3 * sizeof(float));
        GL.EnableVertexAttribArray(1);
    }
}