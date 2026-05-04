using OpenTK.Mathematics;

namespace Task1;

public class Material
{
    public Vector3 Ambient { get; } = new(0.4f, 0.4f, 0.4f);
    public Vector3 Diffuse { get; } = new(1.0f, 0.5f, 0.31f);
    public Vector3 Specular { get; } = new(0.5f, 0.5f, 0.5f);
    public float Shininess => 32.0f;
}