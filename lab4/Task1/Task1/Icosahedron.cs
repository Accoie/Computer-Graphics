using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Task1
{
    public class Icosahedron
    {
        private static readonly float GoldenRatio = (float)((1.0f + Math.Sqrt(5.0f)) / 2f);

        private readonly float[][] _vertices = new float[][]
        {
            [-1f, GoldenRatio, 0f],
            [1f, GoldenRatio, 0f],
            [-1f, -GoldenRatio, 0f],
            [1f, -GoldenRatio, 0f],
            [0f, -1f, GoldenRatio],
            [0f, 1f, GoldenRatio],
            [0f, -1f, -GoldenRatio],
            [0f, 1f, -GoldenRatio],
            [GoldenRatio, 0f, -1f],
            [GoldenRatio, 0f, 1f],
            [-GoldenRatio, 0f, -1f],
            [-GoldenRatio, 0f, 1f]
        };

        private readonly int[][] _sides = new int[][]
        {
            [11, 10, 2],
            [3, 4, 2],
            [8, 6, 7],
            [7, 1, 8],
            [1, 5, 9],
            [0, 11, 5],
            [3, 8, 9],
            [9, 8, 1],
            [10, 7, 6],
            [0, 10, 11],
            [0, 1, 7],
            [3, 2, 6],
            [0, 7, 10],
            [3, 9, 4],
            [6, 2, 10],
            [3, 6, 8],
            [0, 5, 1],
            [5, 11, 4],
            [2, 4, 11],
            [4, 9, 5]
        };

        private readonly int[][] _lines = new int[][]
        {
            [0, 11],
            [0, 5],
            [5, 11],
            [0, 1],
            [1, 5],
            [1, 7],
            [0, 7],
            [0, 10],
            [7, 10],
            [0, 11],
            [10, 11],
            [1, 9],
            [5, 9],
            [4, 5],
            [4, 11],
            [2, 11],
            [2, 10],
            [6, 10],
            [6, 7],
            [1, 8],
            [7, 8],
            [3, 4],
            [3, 9],
            [4, 9],
            [2, 3],
            [2, 4],
            [3, 6],
            [2, 6],
            [3, 8],
            [6, 8],
            [8, 9]
        };

        private readonly Color4[] _sidesColors =
        [
            new(1f, 1f, 1f, 0.7f),   // Белый
            new(1f, 0f, 0f, 0.7f),   // Красный
            new(0f, 1f, 0f, 0.7f),   // Зелёный
            new(0f, 0f, 1f, 0.7f),   // Синий
            new(1f, 1f, 0f, 0.7f),   // Жёлтый
            new(0f, 1f, 1f, 0.7f),   // Голубой
            new(1f, 0f, 1f, 0.7f),   // Пурпурный
            new(0.5f, 0.8f, 0.5f, 0.7f), // Светло-зелёный
            new(0.8f, 0.5f, 0.8f, 0.7f), // Светло-пурпурный
            new(0.4f, 0f, 0.4f, 0.7f)    // Фиолетовый
        ];
        
        private readonly Color4[] _vertexColors =
        [
            new(1f, 0f, 0f, 1f),         // Красный(0)
            new(0f, 1f, 0f, 1f),         // Зелёный(1)
            new(0f, 0f, 1f, 1f),         // Синий(2)
            new(1f, 1f, 0f, 1f),         // Жёлтый(3)
            new(1f, 0f, 1f, 1f),         // Пурпурный(4)
            new(0f, 1f, 1f, 1f),         // Голубой(5)
            new(1f, 0.5f, 0f, 1f),       // Оранжевый(6)
            new(0.5f, 0f, 1f, 1f),       // Фиолетовый(7)
            new(1f, 0.75f, 0.75f, 1f),   // Светло-розовый(8)
            new(0.5f, 1f, 0.5f, 1f),     // Светло-зелёный(9)
            new(1f, 1f, 1f, 1f),         // Белый(10)
            new(0.5f, 0.5f, 0.5f, 1f)    // Серый(11)
        ];
        
        public void Draw()
        {
            DrawVertices();
            DrawLines();
            GL.Enable(EnableCap.CullFace);
            
            GL.CullFace(TriangleFace.Front);
            DrawSides();

            GL.CullFace(TriangleFace.Back);
            DrawSides();
            
            GL.Disable(EnableCap.CullFace);
        }

        private void DrawVertices()
        {
            GL.Disable(EnableCap.Lighting); 
            GL.Enable(EnableCap.PointSmooth);
            GL.Hint(HintTarget.PointSmoothHint, HintMode.Nicest);
            GL.PointSize(20f); 

            GL.Begin(PrimitiveType.Points);

            for (int i = 0; i < _vertices.Length; i++)
            {
                GL.Color4(_vertexColors[i]);
                GL.Vertex3(_vertices[i][0], _vertices[i][1], _vertices[i][2]);
            }

            GL.End();

            GL.Disable(EnableCap.PointSmooth);
            GL.Enable(EnableCap.Lighting); 
        }
        
        private void DrawLines()
        {
            GL.Color4(0f, 0f, 0f, 1f);

            GL.Begin(PrimitiveType.Lines);

            foreach (int[] line in _lines)
            {
                foreach (int vertexIndex in line)
                {
                    GL.Vertex3(_vertices[vertexIndex]);
                }
            }

            GL.End();
        }

        private void DrawSides()
        {
            GL.Begin(PrimitiveType.Triangles);

            int i = 0;
       
            foreach (int[] side in _sides)
            {
                GL.Color4(_sidesColors[i % _sidesColors.Length]);

                float[] arV0 = _vertices[side[0]];
                float[] arV1 = _vertices[side[1]];
                float[] arV2 = _vertices[side[2]];

                Vector3 v0 = new Vector3(arV0[0], arV0[1], arV0[2]);
                Vector3 v1 = new Vector3(arV1[0], arV1[1], arV1[2]);
                Vector3 v2 = new Vector3(arV2[0], arV2[1], arV2[2]);

                Vector3 normal = Vector3.Cross(
                    v1 - v0,
                    v2 - v0);
                
                normal.Normalize();

                GL.Normal3(normal);

                foreach (int vertexIndex in side)
                {
                    GL.Vertex3(_vertices[vertexIndex]);
                }
                i++;
            }

            GL.End();
        }
    }
}