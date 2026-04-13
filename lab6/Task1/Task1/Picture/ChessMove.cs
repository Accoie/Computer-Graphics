using OpenTK.Mathematics;

namespace Task1.Picture;

public class ChessMove
{
    public string Piece { get; set; }
    public (int x, int z) From { get; set; }
    public (int x, int z) To { get; set; }
    public bool IsWhite { get; set; }
    public bool IsCastling { get; set; } = false;
    public bool IsCapture { get; set; } = false;
}