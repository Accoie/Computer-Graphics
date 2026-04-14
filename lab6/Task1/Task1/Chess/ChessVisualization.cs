using OpenTK.Mathematics;
using Task1.Picture;
using Task1.Shaders;
using Task1.TextureService;

namespace Task1.Chess;

public class ChessVisualization
{
    private ChessBoard _board;
    private Dictionary<ChessPiece, Shape> _pieceShapes = new();
    private Shape _boardShape;
    private bool _initialized = false;

    public ChessBoard Board => _board;

    public ChessVisualization()
    {
        _board = new ChessBoard();
    }

    public void Initialize()
    {
        if (_initialized)
            return;


    
        string baseDir = AppDomain.CurrentDomain.BaseDirectory;
        string shapesDir = Path.Combine(baseDir, "shapes");
        string texturesDir = Path.Combine(baseDir, "textures");
    
        int whiteTextureId = TextureLoader.LoadTexture(Path.Combine(texturesDir, "WhitePiece.jpg"));
        int blackTextureId = TextureLoader.LoadTexture(Path.Combine(texturesDir, "BlackPiece.jpg"));
    
        _board.Initialize();
        _board.SetupGameSequence();


        _boardShape = new Shape(0f, 0f, 0f, 4f);
        _boardShape.LoadPicture(Path.Combine(shapesDir, "Board.3ds"));
        _boardShape.SetTexture(whiteTextureId);


        foreach (var piece in _board.GetAllPieces())
        {
            var shape = new Shape(piece.Position.X, piece.Position.Y, piece.Position.Z, 0.09f);
            string modelFile = piece.GetModelPath();
            string modelPath = Path.Combine(shapesDir, Path.GetFileName(modelFile));
            shape.LoadPicture(modelPath);
        
            if (piece.Color == PieceColor.White)
            {
                shape.SetTexture(whiteTextureId);
            }
            else
            {
                shape.SetTexture(blackTextureId);
            }
        
            _pieceShapes[piece] = shape;
        }


        _initialized = true;
    }

    public void Update(float deltaTime)
    {
        if (!_initialized)
            return;

        _board.Update(deltaTime);

        foreach (var piece in _board.GetAllPieces())
        {
            if (_pieceShapes.TryGetValue(piece, out var shape))
            {
                shape.X = piece.Position.X;
                shape.Y = piece.Position.Y;
                shape.Z = piece.Position.Z;
            }
        }
    }

    public void Paint(Shader shader)
    {
        if (!_initialized)
            return;

        _boardShape.Paint(shader);

        foreach (var piece in _board.GetAllPieces())
        {
            if (_pieceShapes.TryGetValue(piece, out var shape))
            {
                shape.Paint(shader);
            }
        }
    }

    public void Dispose()
    {
        _boardShape?.Dispose();
        foreach (var shape in _pieceShapes.Values)
        {
            shape.Dispose();
        }
    }

    public bool IsGameFinished()
    {
        return _board.IsGameFinished();
    }

    public string GetGameStatus()
    {
        int whitePieces = _board.GetWhitePieceCount();
        int blackPieces = _board.GetBlackPieceCount();

        if (_board.IsGameFinished())
        {
            if (whitePieces > blackPieces)
                return $"Белые выигрывают! Белые: {whitePieces} фигур, Чёрные: {blackPieces} фигур";
            else if (blackPieces > whitePieces)
                return $"Чёрные выигрывают! Белые: {whitePieces} фигур, Чёрные: {blackPieces} фигур";
            else
                return $"Ничья! Белые: {whitePieces} фигур, Чёрные: {blackPieces} фигур";
        }

        return $"Игра идёт... Белые: {whitePieces} фигур, Чёрные: {blackPieces} фигур";
    }
}






