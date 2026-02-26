using OpenTK.Mathematics;
using Task2.Strategies;

namespace Task2.Components;

public class Fish
{
    public Vector2 Position { get; set; }
    public Vector2 Size { get; set; }
    public float Offset { get; set; }
    private readonly IRenderStrategy<Fish> _renderStrategy;

    public Fish(Vector2 pos, Vector2 size, float offset, IRenderStrategy<Fish> strategy)
    {
        Position = pos;
        Size = size;
        Offset = offset;
        _renderStrategy = strategy;
    }

    public void Draw()
    {
        _renderStrategy.Render(this);
    }
}