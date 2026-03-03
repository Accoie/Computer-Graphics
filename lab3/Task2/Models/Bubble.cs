using OpenTK.Mathematics;

namespace Task2.Models;

public class Bubble(Vector2 position, Vector2 speed, Vector2 direction, float size = 8.0f)
{
    public Vector2 Position = position;
    public Vector2 Speed = speed;
    public Vector2 Direction = direction;
    public readonly float Size = size;
    public float LifeTime = 5.0f;
}
