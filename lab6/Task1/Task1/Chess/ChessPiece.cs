using OpenTK.Mathematics;

namespace Task1.Chess;

public enum PieceType
{
    Pawn,
    Rook,
    Knight,
    Bishop,
    Queen,
    King
}

public enum PieceColor
{
    White,
    Black
}

public class ChessPiece
{
    public PieceType Type { get; set; }
    public PieceColor Color { get; set; }
    public int File { get; set; }
    public int Rank { get; set; }
    
    public Vector3 Position { get; set; }
    public Vector3 TargetPosition { get; set; }
    public bool IsAnimating { get; set; }
    public float AnimationProgress { get; set; }

    public const float Gap = 1.6f;
    
    public ChessPiece(PieceType type, PieceColor color, int file, int rank)
    {
        Type = type;
        Color = color;
        File = file;
        Rank = rank;
        Position = GetPositionFromCoordinates(file, rank);
        TargetPosition = Position;
        IsAnimating = false;
        AnimationProgress = 0f;
    }

    public static Vector3 GetPositionFromCoordinates(int file, int rank)
    {
        float x = (file - 3.5f) * Gap;
        float y = (rank - 3.5f) * Gap;
        float z = 0.2f;
        return new Vector3(x, y, z);
    }

    public static (int file, int rank) GetCoordinatesFromPosition(Vector3 pos)
    {
        int file = (int)Math.Round(pos.X / 0.8f + 3.5f);
        int rank = (int)Math.Round(pos.Y / 0.8f + 3.5f);
        return (file, rank);
    }

    public void MoveTo(int targetFile, int targetRank, float animationDuration = 0.5f)
    {
        File = targetFile;
        Rank = targetRank;
        TargetPosition = GetPositionFromCoordinates(targetFile, targetRank);
        IsAnimating = true;
        AnimationProgress = 0f;
    }

    public void UpdateAnimation(float deltaTime, float animationDuration = 0.5f)
    {
        if (!IsAnimating)
            return;

        AnimationProgress += deltaTime / animationDuration;

        if (AnimationProgress >= 1f)
        {
            Position = TargetPosition;
            IsAnimating = false;
            AnimationProgress = 1f;
        }
        else
        {
            Position = Vector3.Lerp(Position, TargetPosition, AnimationProgress);
        }
    }

    public void MoveToPosition(Vector3 targetPos, float animationDuration = 0.5f)
    {
        TargetPosition = targetPos;
        IsAnimating = true;
        AnimationProgress = 0f;
    }

    public string GetModelPath()
    {
        return Type switch
        {
            PieceType.Pawn => "shapes/Pawn.3ds",
            PieceType.Rook => "shapes/Rook.3ds",
            PieceType.Knight => "shapes/Knight.3ds",
            PieceType.Bishop => "shapes/Bishop.3ds",
            PieceType.Queen => "shapes/Queen.3ds",
            PieceType.King => "shapes/King.3ds",
            _ => throw new ArgumentException("Unknown piece type")
        };
    }
}

