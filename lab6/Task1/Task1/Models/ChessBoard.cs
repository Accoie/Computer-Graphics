using OpenTK.Mathematics;
using Task1.Models.Data;
using Task1.Models.Enums;

namespace Task1.Models;

public class ChessBoard
{
    private readonly Dictionary<string, ChessPiece> _pieces = new();
    private readonly Dictionary<string, PieceInfo> _initialPieceData = new();
    private List<ChessMove> _moves = new();
    private int _currentMoveIndex;
    private readonly float _timeBetweenMoves = 1.0f;
    private float _moveTimer;
    private readonly float _animationDuration = 2.5f;
    private const  float Gap = 1.6f;
    private ChessMove[] _originalMoves = Array.Empty<ChessMove>();
    private float _restartDelay = 3.0f;
    private float _restartTimer;
    private bool _gameFinished;

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
        SetupWhiteBackRank();
        SetupWhitePawns();
        SetupBlackPawns();
        SetupBlackBackRank();
    }

    private void SetupWhiteBackRank()
    {
        PieceInfo[] whitePieces = new[]
        {
            new PieceInfo(file: 0, rank: 0, type: PieceType.Rook, color: PieceColor.White, id: "WR1"),
            new PieceInfo(file: 1, rank: 0, type: PieceType.Knight, color: PieceColor.White, id: "WN1"),
            new PieceInfo(file: 2, rank: 0, type: PieceType.Bishop, color: PieceColor.White, id: "WB1"),
            new PieceInfo(file: 3, rank: 0, type: PieceType.Queen, color: PieceColor.White, id: "WQ"),
            new PieceInfo(file: 4, rank: 0, type: PieceType.King, color: PieceColor.White, id: "WK"),
            new PieceInfo(file: 5, rank: 0, type: PieceType.Bishop, color: PieceColor.White, id: "WB2"),
            new PieceInfo(file: 6, rank: 0, type: PieceType.Knight, color: PieceColor.White, id: "WN2"),
            new PieceInfo(file: 7, rank: 0, type: PieceType.Rook, color: PieceColor.White, id: "WR2"),
        };

        foreach (PieceInfo p in whitePieces)
        {
            AddPiece(p.Type, PieceColor.White, p.File, p.Rank, p.Id);
        }
    }

    private void SetupBlackBackRank()
    {
        PieceInfo[] blackPieces = new[]
        {
            new PieceInfo(file: 0, rank: 7, type: PieceType.Rook, color: PieceColor.Black, id: "BR1"),
            new PieceInfo(file: 1, rank: 7, type: PieceType.Knight, color: PieceColor.Black, id: "BN1"),
            new PieceInfo(file: 2, rank: 7, type: PieceType.Bishop, color: PieceColor.Black, id: "BB1"),
            new PieceInfo(file: 3, rank: 7, type: PieceType.Queen, color: PieceColor.Black, id: "BQ"),
            new PieceInfo(file: 4, rank: 7, type: PieceType.King, color: PieceColor.Black, id: "BK"),
            new PieceInfo(file: 5, rank: 7, type: PieceType.Bishop, color: PieceColor.Black, id: "BB2"),
            new PieceInfo(file: 6, rank: 7, type: PieceType.Knight, color: PieceColor.Black, id: "BN2"),
            new PieceInfo(file: 7, rank: 7, type: PieceType.Rook, color: PieceColor.Black, id: "BR2"),
        };

        foreach (PieceInfo p in blackPieces)
        {
            AddPiece(p.Type, PieceColor.Black, p.File, p.Rank, p.Id);
        }
    }

    private void SetupWhitePawns()
    {
        for (int file = 0; file < 8; file++)
        {
            AddPiece(PieceType.Pawn, PieceColor.White, file, 1, $"WP{file}");
        }
    }

    private void SetupBlackPawns()
    {
        for (int file = 0; file < 8; file++)
        {
            AddPiece(PieceType.Pawn, PieceColor.Black, file, 6, $"BP{file}");
        }
    }

    private void AddPiece(PieceType type, PieceColor color, int file, int rank, string id)
    {
        float x = (file - 3.5f) * Gap;
        float y = (rank - 3.5f) * Gap;
        float z = 0.2f;
        
        ChessPiece piece = new ChessPiece(type, color, file, rank)
        {
            Position = new Vector3(x, y, z),
            TargetPosition = new Vector3(x, y, z)
        };
        
        _pieces[id] = piece;
        _initialPieceData[id] = new PieceInfo(file, rank, type, color, id);
    }

    public void SetupGameSequence(ChessMove[] moves)
    {
        _originalMoves = moves;
        _moves = moves.ToList();
    
        _currentMoveIndex = 0;
        _moveTimer = 0f;
        _restartTimer = 0f;
        _gameFinished = false;
    }   
    
    public void Update(float deltaTime)
    {
        UpdatePieceAnimations(deltaTime);
    
        bool boardReady = !_pieces.Values.Any(p => p.IsAnimating);
        if (boardReady)
        {
            if (_currentMoveIndex < _moves.Count)
            {
                HandleMoveSequence(deltaTime);
            }
            else
            {
                HandleGameCompletion(deltaTime);
            }
        }
    }

    private void HandleMoveSequence(float deltaTime)
    {
        _moveTimer += deltaTime;
        if (_moveTimer >= _timeBetweenMoves)
        {
            ExecuteNextMove();
            _moveTimer = 0f;
        }
    }

    private void HandleGameCompletion(float deltaTime)
    {
        if (!_gameFinished)
        {
            _gameFinished = true;
            _restartTimer = 0f;
            
            return;
        }

        _restartTimer += deltaTime;
        if (_restartTimer >= _restartDelay)
        {
            RestartGame();
        }
    }
    
    private void UpdatePieceAnimations(float deltaTime)
    {
        foreach (ChessPiece piece in _pieces.Values.Where(p => p.IsAnimating))
        {
            piece.UpdateAnimation(deltaTime, _animationDuration);
        }
    }
    private void RestartGame()
    {
        foreach (KeyValuePair<string, ChessPiece> kvp in _pieces)
        {
            if (_initialPieceData.TryGetValue(kvp.Key, out PieceInfo initialData))
            {
                kvp.Value.ResetToInitialPosition(initialData.File, initialData.Rank);
            }
        }
        
        foreach (KeyValuePair<string, PieceInfo> kvp in _initialPieceData)
        {
            if (!_pieces.ContainsKey(kvp.Key))
            {
                AddPiece(kvp.Value.Type, kvp.Value.Color, kvp.Value.File, kvp.Value.Rank, kvp.Key);
            }
        }
        
        _moves = _originalMoves.ToList();
        _currentMoveIndex = 0;
        _moveTimer = 0f;
        _restartTimer = 0f;
        _gameFinished = false;
    }

    private void ExecuteNextMove()
    {
        if (_currentMoveIndex >= _moves.Count)
        {
            return;
        }

        ChessMove move = _moves[_currentMoveIndex];

        ChessPiece? piece = _pieces.Values.FirstOrDefault(p => p.File == move.FromFile && p.Rank == move.FromRank);

        if (piece != null)
        {
            string pieceColor = piece.Color == PieceColor.White ? "White" : "Black";
            Console.WriteLine($"\nMove {_currentMoveIndex + 1}: {pieceColor} {piece.Type}");
            Console.WriteLine($"  From: File={move.FromFile}, Rank={move.FromRank}");
            Console.WriteLine($"  To: File={move.ToFile}, Rank={move.ToRank}");
            
            ChessPiece? capturedPiece = _pieces.Values.FirstOrDefault(p => p.File == move.ToFile && p.Rank == move.ToRank);
            if (capturedPiece != null)
            {
                Console.WriteLine($"  CAPTURED: {capturedPiece.Color} {capturedPiece.Type}");
                _pieces.Remove(_pieces.FirstOrDefault(x => x.Value == capturedPiece).Key);
            }

            piece.MoveTo(move.ToFile, move.ToRank);
            _currentMoveIndex++;
        }
    }
}