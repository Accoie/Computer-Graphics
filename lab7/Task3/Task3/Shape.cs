using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Task3
{
    public class Shape
    {
        private int _vao;
        private int _vbo;
        private int _vertexCount;

        public Shape(Shader shader, int rows = 100, int cols = 100)
        {
            float[] vertices = GeneratePlaneGrid(rows, cols);
            _vertexCount = vertices.Length / 3;
            
            CreateAndSetupBuffers(shader, vertices);
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
        
        private float[] GeneratePlaneGrid(int rows, int cols)
        {
            List<float> vertices = new List<float>();
            
            for (int row = 0; row < rows - 1; row++)
            {
                for (int col = 0; col < cols - 1; col++)
                {
                    Cell cell = GenerateCellVertices(row, col, rows, cols);
                    AddTrianglesFromCell(vertices, cell);
                }
            }
            
            return vertices.ToArray();
        }

        private Cell GenerateCellVertices(int row, int col, int rows, int cols)
        {
            float x0 = NormalizeCoordinate(row, rows);
            float y0 = NormalizeCoordinate(col, cols);
            float x1 = NormalizeCoordinate(row + 1, rows);
            float y1 = y0;
            float x2 = x0;
            float y2 = NormalizeCoordinate(col + 1, cols);
            float x3 = x1;
            float y3 = y2;

            return new Cell(
                new Vector2(x0, y0), 
                new Vector2(x1, y1),
                new Vector2(x2, y2), 
                new Vector2(x3, y3) 
            );
        }

        private float NormalizeCoordinate(int index, int maxIndex)
        {
            return -1.0f + 2.0f * index / (maxIndex - 1);
        }
        
        private void AddTrianglesFromCell(List<float> vertices, Cell cell)
        {
            AddTriangle(vertices, 
                cell.TopLeft, 
                cell.TopRight, 
                cell.BottomLeft);
            
            AddTriangle(vertices, 
                cell.TopRight, 
                cell.BottomRight, 
                cell.BottomLeft);
        }
        
        private void AddTriangle(List<float> vertices, Vector2 v1, Vector2 v2, Vector2 v3)
        {
            vertices.Add(v1.X);
            vertices.Add(v1.Y);
            vertices.Add(0f);
            vertices.Add(v2.X);
            vertices.Add(v2.Y);
            vertices.Add(0f);
            vertices.Add(v3.X); 
            vertices.Add(v3.Y);
            vertices.Add(0f);
        }
        
        private void CreateAndSetupBuffers(Shader shader, float[] vertices)
        {
            _vao = GL.GenVertexArray();
            GL.BindVertexArray(_vao);

            _vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, 
                vertices.Length * sizeof(float), 
                vertices, 
                BufferUsageHint.StaticDraw);

            SetupVertexAttributes(shader);
            
            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        private void SetupVertexAttributes(Shader shader)
        {
            int posLoc = shader.GetAttribLocation("position");
            
            GL.EnableVertexAttribArray(posLoc);
            
            int stride = 3 * sizeof(float);
            int offset = 0;
            GL.VertexAttribPointer(posLoc, 3, VertexAttribPointerType.Float, false, stride, offset);
        }
        
        private struct Cell
        {
            public Vector2 TopLeft;
            public Vector2 TopRight;
            public Vector2 BottomLeft;
            public Vector2 BottomRight;

            public Cell(Vector2 tl, Vector2 tr, Vector2 bl, Vector2 br)
            {
                TopLeft = tl;
                TopRight = tr;
                BottomLeft = bl;
                BottomRight = br;
            }
        }
    }
}