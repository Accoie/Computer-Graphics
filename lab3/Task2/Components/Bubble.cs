using OpenTK.Mathematics;

namespace Task2.Components;

public class Bubble
{
    public Vector2 Position;
    public Vector2 Speed;
    public Vector2 Direction;
    public float Size;
    public float LifeTime;

    public Bubble( Vector2 position, Vector2 speed, Vector2 direction, float size = 8.0f )
    {
        Position = position;
        Speed = speed;
        Direction = direction;
        Size = size;
        LifeTime = 5.0f;
    }
}
