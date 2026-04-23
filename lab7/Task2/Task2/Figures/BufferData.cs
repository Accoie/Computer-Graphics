using OpenTK.Graphics.OpenGL;

namespace Task2.Figures;

public struct BufferData
{
    public int Vao;
    public PrimitiveType PrimitiveType;
    public int VertexCount;

    public BufferData(int vao, PrimitiveType primitiveType, int vertexCount)
    {
        Vao = vao;
        PrimitiveType = primitiveType;
        VertexCount = vertexCount;
    }
}