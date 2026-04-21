using OpenTK.Graphics.OpenGL4;

namespace Task1
{
    public class Canabola
    {
        private int _vao;
        private int _vbo;
        private int _vertexCount;

        public Canabola(float startAngle, float endAngle, float step)
        {
            _vertexCount = (int)((endAngle - startAngle) / step);

            _vao = GL.GenVertexArray();
            GL.BindVertexArray(_vao);

            _vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);

            float[] vertices = new float[_vertexCount];
            for (int i = 0; i < _vertexCount; i++)
            {
                vertices[i] = startAngle + i * step;
            }

            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 1, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(0);

            GL.BindVertexArray(0);
        }

        public void Draw()
        {
            GL.BindVertexArray(_vao);
            GL.DrawArrays(PrimitiveType.LineStrip, 0, _vertexCount);
            GL.BindVertexArray(0);
        }

        public void Dispose()
        {
            GL.DeleteBuffer(_vbo);
            GL.DeleteVertexArray(_vao);
        }
    }
}
