namespace Task2.Scenes;

public struct BufferData
{
    public readonly int Vao;
    public int VertexCount;

    public BufferData(int vao, int vertexCount)
    {
        Vao = vao;
        VertexCount = vertexCount;
    }
}
