namespace Task3.Shapes;

public struct BufferData
{
    public readonly int Vao;
    public readonly int VertexCount;

    public BufferData(int vao, int vertexCount)
    {
        Vao = vao;
        VertexCount = vertexCount;
    }
}
