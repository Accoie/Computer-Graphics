using OpenTK.Mathematics;
using Task2.Strategies;

namespace Task2.Components;

public class Fish
{
    public Vector2 Position { get; set; }
    public Vector2 Size { get; set; }
    public float Offset { get; set; }
    private readonly IDrawingStrategy<Fish> _drawingStrategy;

    public Fish(Vector2 pos, Vector2 size, float offset, IDrawingStrategy<Fish> strategy)
    {
        Position = pos;
        Size = size;
        Offset = offset;
        _drawingStrategy = strategy;
    }

    public void Draw()
    {
        _drawingStrategy.Draw(this);
    }
}