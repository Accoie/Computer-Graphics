namespace Task1.Models.Data;

public struct ChessMove
{
    public int FromFile { get; }
    public int FromRank { get; }
    public int ToFile { get; }
    public int ToRank { get; }

    public ChessMove(int fromFile, int fromRank, int toFile, int toRank)
    {
        FromFile = fromFile;
        FromRank = fromRank;
        ToFile = toFile;
        ToRank = toRank;
    }
}