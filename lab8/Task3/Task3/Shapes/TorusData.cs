using OpenTK.Mathematics;

namespace Task3.Shapes;

public struct TorusData
{
    public Vector3 Position;
    public readonly float MajorRadius; 
    public readonly float MinorRadius;  
    public Matrix4 ModelMatrix;

    public TorusData(Vector3 position, float majorRadius, float minorRadius)
    {
        Position = position;
        MajorRadius = majorRadius;
        MinorRadius = minorRadius;
        ModelMatrix = Matrix4.CreateTranslation(position);
    }
}

