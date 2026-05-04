using OpenTK.Mathematics;

namespace Task1;

public class Light
{
    public Vector3 Position { get; set; } = new(2.0f, 2.0f, 2.0f);
    public Vector3 Ambient { get; } = new(0.4f, 0.4f, 0.4f);
    public Vector3 Diffuse { get; } = new(1.0f, 1.0f, 1.0f);
    public Vector3 Specular { get; } = new(1.0f, 1.0f, 1.0f);
}