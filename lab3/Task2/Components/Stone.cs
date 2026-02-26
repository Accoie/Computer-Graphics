using OpenTK.Mathematics;
using Task2.Strategies;

namespace Task2.Components;

public class Stone : IRenderable
{
    public Vector2 Position { get; set; }
    public Vector2 Size { get; set; }
    private readonly IRenderStrategy<Stone> _renderStrategy;

    public Stone(Vector2 pos, Vector2 size, IRenderStrategy<Stone> strategy)
    {
        Position = pos;
        Size = size;
        _renderStrategy = strategy;
    }

    public void Draw()
    {
        _renderStrategy.Render(this);
    }
}

public interface IRenderable
{
}