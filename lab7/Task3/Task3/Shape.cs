using OpenTK.Graphics.OpenGL4;

namespace Task3
{
    public class Shape
    {
        private int _vao;
        private int _vbo;
        private int _vertexCount;

        public int VertexCount => _vertexCount;

        public Shape(Shader shader, int rows = 100, int cols = 100)
        {
            GenerateSurfaceMesh(shader, rows, cols);
        }

        private void GenerateSurfaceMesh(Shader shader, int rows, int cols)
        {
            List<float> vertices = new List<float>();

            for (int i = 0; i < rows - 1; i++)
            {
                for (int j = 0; j < cols - 1; j++)
                {
                    float x0 = -1.0f + 2.0f * i / (rows - 1);
                    float y0 = -1.0f + 2.0f * j / (cols - 1);
                    float x1 = -1.0f + 2.0f * (i + 1) / (rows - 1);
                    float y1 = y0;
                    float x2 = x0;
                    float y2 = -1.0f + 2.0f * (j + 1) / (cols - 1);
                    float x3 = x1;
                    float y3 = y2;

                    AddTriangle(vertices, x0, y0, x1, y1, x2, y2);
                    AddTriangle(vertices, x1, y1, x3, y3, x2, y2);
                }
            }

            _vertexCount = vertices.Count / 3;

            _vao = GL.GenVertexArray();
            GL.BindVertexArray(_vao);

            _vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Count * sizeof(float), vertices.ToArray(), BufferUsageHint.StaticDraw);

            int posLoc = shader.GetAttribLocation("position");
            if (posLoc == -1)
            {
                Console.WriteLine("WARNING: 'position' attribute not found!");
            }
            else
            {
                GL.EnableVertexAttribArray(posLoc);
                GL.VertexAttribPointer(posLoc, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            }

            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        private void AddTriangle(List<float> v, float x0, float y0, float x1, float y1, float x2, float y2)
        {
            v.Add(x0); v.Add(y0); v.Add(0f);
            v.Add(x1); v.Add(y1); v.Add(0f);
            v.Add(x2); v.Add(y2); v.Add(0f);
        }

        public void Draw()
        {
            GL.BindVertexArray(_vao);
            GL.DrawArrays(PrimitiveType.Triangles, 0, _vertexCount);
            GL.BindVertexArray(0);
        }

        public void Dispose()
        {
            GL.DeleteBuffer(_vbo);
            GL.DeleteVertexArray(_vao);
        }
    }
}
