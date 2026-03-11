namespace Task3.Models;

public readonly struct Block(int x, int y)
{
    public int X { get; } = x;
    public int Y { get; } = y;

    public Block Shift(int dx, int dy) => new(X + dx, Y + dy);
}