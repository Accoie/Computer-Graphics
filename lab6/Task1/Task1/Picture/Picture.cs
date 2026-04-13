using Task1.Shaders;

namespace Task1.Picture;

public class Picture
{
    List<(Shape shape, string file)> _shapes =
    [
        (new(0f, 0f, 0f, 4f), "shapes/Board.3ds"),
        (new(0.8f, 5.6f, 0.2f, 0.09f),"shapes/King.3ds"),
        (new(-2.4f, -5.6f, 0.2f, 0.09f, true), "shapes/King.3ds"),
        (new(-0.8f, -5.6f, 0.2f, 0.09f, true), "shapes/Queen.3ds"),
        (new(0.8f, 4.0f, 0.2f, 0.09f), "shapes/Queen.3ds"),
        (new(4.0f, 2.4f, 0.2f, 0.09f), "shapes/Bishop.3ds"),
        (new(-4.0f, 0.8f, 0.2f, 0.09f), "shapes/Rook.3ds"),
        (new(5.6f, -4.0f, 0.2f, 0.09f, true), "shapes/Rook.3ds"),
        (new(5.6f, -2.4f, 0.2f, 0.09f, true), "shapes/Pawn.3ds"),
        (new(-2.4f, -2.4f, 0.2f, 0.09f, true), "shapes/Pawn.3ds"),
        (new(-4.0f, -4.0f, 0.2f, 0.09f, true), "shapes/Pawn.3ds"),
        (new(-0.8f, 4.0f, 0.2f, 0.09f), "shapes/Pawn.3ds"),
        (new(2.4f, 2.4f, 0.2f, 0.09f), "shapes/Pawn.3ds"),
        (new(5.6f, 5.6f, 0.2f, 0.09f), "shapes/Rook.3ds"),
    ];

    public void LoadPicture()
    {
        _shapes.ForEach(s => s.shape.LoadPicture(s.file));
    }
    public void Paint(Shader shader)
    {
        _shapes.ForEach(s => s.shape.Paint(shader));
    }
}