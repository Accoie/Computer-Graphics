using OpenTK.Mathematics;
using Task2.Strategies;

namespace Task2.Components;

public class Stone
{
    public Vector2 Position { get; set; }
    public Vector2 Size { get; set; }
    private readonly IDrawingStrategy<Stone> _drawingStrategy;

    public Stone(Vector2 pos, Vector2 size, IDrawingStrategy<Stone> strategy)
    {
        Position = pos;
        Size = size;
        _drawingStrategy = strategy;
    }

    public void Draw()
    {
        _drawingStrategy.Draw(this);
    }
}