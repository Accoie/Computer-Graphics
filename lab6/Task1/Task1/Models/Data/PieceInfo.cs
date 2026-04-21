using Task1.Models.Enums;

namespace Task1.Models.Data;

public struct PieceInfo
{
    public int File { get; }
    public int Rank { get; }
    public PieceType Type { get; }
    public PieceColor Color { get; }
    public string Id { get; }

    public PieceInfo(int file, int rank, PieceType type, PieceColor color, string id)
    {
        File = file;
        Rank = rank;
        Type = type;
        Color = color;
        Id = id;
    }
}