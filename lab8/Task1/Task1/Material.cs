using OpenTK.Mathematics;

namespace Task1;

public class Material
{
    public Vector3 Ambient { get; } = new(0.4f, 0.0f, 0.4f);
    public Vector3 Diffuse { get; } = new(0.2f, 0.5f, 0.8f);
    public Vector3 Specular { get; } = new(1f, 0f, 0f);
    public float Shininess => 32.0f;
}