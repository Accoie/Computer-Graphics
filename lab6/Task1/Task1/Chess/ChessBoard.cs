using OpenTK.Mathematics;

namespace Task1.Chess;

public class ChessBoard
{
    private Dictionary<string, ChessPiece> _pieces = new();
    private List<(int fromFile, int fromRank, int toFile, int toRank)> _moves = new();
    private int _currentMoveIndex = 0;
    private float _timeBetweenMoves = 2f;
    private float _moveTimer = 0f;
    private float _animationDuration = 0.8f;
    private const  float Gap = 1.6f;

    public List<ChessPiece> GetAllPieces()
    {
        return _pieces.Values.ToList();
    }

    public void Initialize()
    {
        SetupInitialPosition();
    }

    private void SetupInitialPosition()
    {
        _pieces.Clear();
        var whitePieces = new[]
        {
            (file: 0, rank: 0, type: PieceType.Rook, id: "WR1"),
            (file: 1, rank: 0, type: PieceType.Knight, id: "WN1"),
            (file: 2, rank: 0, type: PieceType.Bishop, id: "WB1"),
            (file: 3, rank: 0, type: PieceType.Queen, id: "WQ"),
            (file: 4, rank: 0, type: PieceType.King, id: "WK"),
            (file: 5, rank: 0, type: PieceType.Bishop, id: "WB2"),
            (file: 6, rank: 0, type: PieceType.Knight, id: "WN2"),
            (file: 7, rank: 0, type: PieceType.Rook, id: "WR2"),
        };

        foreach (var p in whitePieces)
        {
            float x = (p.file - 3.5f) * Gap;
            float y = (p.rank - 3.5f) * Gap;
            float z = 0.2f;
            var piece = new ChessPiece(p.type, PieceColor.White, p.file, p.rank)
            {
                Position = new Vector3(x, y, z),
                TargetPosition = new Vector3(x, y, z)
            };
            _pieces[p.id] = piece;
        }

        for (int file = 0; file < 8; file++)
        {
            float x = (file - 3.5f) * Gap;
            float y = (1 - 3.5f) * Gap;
            float z = 0.2f;
            var piece = new ChessPiece(PieceType.Pawn, PieceColor.White, file, 1)
            {
                Position = new Vector3(x, y, z),
                TargetPosition = new Vector3(x, y, z)
            };
            _pieces[$"WP{file}"] = piece;

        }

        for (int file = 0; file < 8; file++)
        {
            float x = (file - 3.5f) * Gap;
            float y = (6 - 3.5f) * Gap;
            float z = 0.2f;
            var piece = new ChessPiece(PieceType.Pawn, PieceColor.Black, file, 6)
            {
                Position = new Vector3(x, y, z),
                TargetPosition = new Vector3(x, y, z)
            };
            _pieces[$"BP{file}"] = piece;

        }

        var blackPieces = new[]
        {
            (file: 0, rank: 7, type: PieceType.Rook, id: "BR1"),
            (file: 1, rank: 7, type: PieceType.Knight, id: "BN1"),
            (file: 2, rank: 7, type: PieceType.Bishop, id: "BB1"),
            (file: 3, rank: 7, type: PieceType.Queen, id: "BQ"),
            (file: 4, rank: 7, type: PieceType.King, id: "BK"),
            (file: 5, rank: 7, type: PieceType.Bishop, id: "BB2"),
            (file: 6, rank: 7, type: PieceType.Knight, id: "BN2"),
            (file: 7, rank: 7, type: PieceType.Rook, id: "BR2"),
        };

        foreach (var p in blackPieces)
        {
            float x = (p.file - 3.5f) * Gap;
            float y = (p.rank - 3.5f) * Gap;
            float z = 0.2f;
            var piece = new ChessPiece(p.type, PieceColor.Black, p.file, p.rank)
            {
                Position = new Vector3(x, y, z),
                TargetPosition = new Vector3(x, y, z)
            };
            _pieces[p.id] = piece;
        }
    }

    public void SetupGameSequence()
    {
        // Детский мат
        _moves = new()
        {
            // 1. e4 e5
            (4, 1, 4, 3),  // White Pawn e2-e4
            (4, 6, 4, 4),  // Black Pawn e7-e5

            // 2. Bc4 Nc6
            (5, 0, 2, 3),  // White Bishop f1-c4
            (1, 7, 2, 5),  // Black Knight b8-c6

            // 3. Qh5 Nf6
            (3, 0, 7, 4),  // White Queen d1-h5
            (6, 7, 5, 5),  // Black Knight g8-f6

            // 4. Qxf7#
            (7, 4, 5, 6),  
        };
    
        _currentMoveIndex = 0;
        _moveTimer = 0f;
    }   
    
    public void Update(float deltaTime)
    {
        foreach (var piece in _pieces.Values)
        {
            if (piece.IsAnimating)
            {
                piece.UpdateAnimation(deltaTime, _animationDuration);
            }
        }

        if (_currentMoveIndex < _moves.Count)
        {
            _moveTimer += deltaTime;

            if (_moveTimer >= _timeBetweenMoves && !IsAnyPieceAnimating())
            {
                ExecuteNextMove();
                _moveTimer = 0f;
            }
        }
    }

    private void ExecuteNextMove()
    {
        if (_currentMoveIndex >= _moves.Count)
            return;

        var (fromFile, fromRank, toFile, toRank) = _moves[_currentMoveIndex];

        var piece = _pieces.Values.FirstOrDefault(p => p.File == fromFile && p.Rank == fromRank);

        if (piece != null)
        {
            string pieceColor = piece.Color == PieceColor.White ? "White" : "Black";
            Console.WriteLine($"\nMove {_currentMoveIndex + 1}: {pieceColor} {piece.Type}");
            Console.WriteLine($"  From: File={fromFile}, Rank={fromRank}");
            Console.WriteLine($"  To: File={toFile}, Rank={toRank}");
            
            var capturedPiece = _pieces.Values.FirstOrDefault(p => p.File == toFile && p.Rank == toRank);
            if (capturedPiece != null)
            {
                Console.WriteLine($"  CAPTURES: {capturedPiece.Color} {capturedPiece.Type}");
                _pieces.Remove(_pieces.FirstOrDefault(x => x.Value == capturedPiece).Key);
            }

            piece.MoveTo(toFile, toRank, _animationDuration);
            _currentMoveIndex++;
        }
    }

    private bool IsAnyPieceAnimating()
    {
        return _pieces.Values.Any(p => p.IsAnimating);
    }

    public bool IsGameFinished()
    {
        bool finished = _currentMoveIndex >= _moves.Count && !IsAnyPieceAnimating();
        if (finished && _currentMoveIndex == _moves.Count)
        {
            int whiteCount = GetWhitePieceCount();
            int blackCount = GetBlackPieceCount();
            Console.WriteLine($"\n=== GAME FINISHED ===");
            Console.WriteLine($"Scholar's Mate achieved!");
            Console.WriteLine($"White pieces remaining: {whiteCount}");
            Console.WriteLine($"Black pieces remaining: {blackCount}");
            Console.WriteLine($"White wins with checkmate!");
            _currentMoveIndex++; // Prevent duplicate output
        }
        return finished;
    }

    public int GetWhitePieceCount()
    {
        return _pieces.Values.Count(p => p.Color == PieceColor.White);
    }

    public int GetBlackPieceCount()
    {
        return _pieces.Values.Count(p => p.Color == PieceColor.Black);
    }
}

