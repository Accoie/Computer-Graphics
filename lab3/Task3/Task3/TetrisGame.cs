using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL;
using Task3.Configs;

namespace Task3
{
    public class TetrisGame : GameWindow
    {
        private readonly Board _board;
        private readonly Shape _currentShape;
        private readonly InputHandler _inputHandler;
        private readonly Renderer _renderer;
        
        private int _oldScore;
        private int _score;
        private string _scoreStr = "0";
        
        private int _level = 1;
        private int _linesCleared;
        private int _linesNeeded;
        
        private bool _isGameOver;
        private bool _isPaused;
        
        private double _fallTimer;
        private double _fastDropTimer;
        private double _currentFallInterval;
        
        private ShapeType _nextShapeType;
        private int _nextShapeColor;

        public TetrisGame() : base(
            new GameWindowSettings 
            { 
                UpdateFrequency = 60.0
            }, 
            new NativeWindowSettings 
            { 
                ClientSize = new Vector2i(GameConfig.WindowWidth, GameConfig.WindowHeight),
                Title = GameConfig.WindowTitle,
                Flags = ContextFlags.Default,
                Profile = ContextProfile.Compatability,
                APIVersion = new Version(3, 3)
            })
        {
            _board = new Board();
            _currentShape = new Shape();
            _inputHandler = new InputHandler();
            _renderer = new Renderer(ClientSize.X, ClientSize.Y);
            
            InitializeLevel();
            GenerateNextPiece();
            _currentShape.SpawnNew();
        }

        private void InitializeLevel()
        {
            _linesNeeded = GameConfig.BaseLinesPerLevel + (_level - 1) * GameConfig.LinesPerLevelIncrease;
            _currentFallInterval = CalculateFallInterval();
        }
        
        private double CalculateFallInterval()
        {
            double interval = GameConfig.BaseFallInterval - (_level - 1) * GameConfig.FallIntervalDecrease;
            return Math.Max(interval, GameConfig.MinFallInterval);
        }

        private void GenerateNextPiece()
        {
            (_nextShapeType, _nextShapeColor) = Shape.GenerateNext();
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            _renderer.Initialize();
            GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
        }
        
        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
            _renderer.UpdateWindowSize(ClientSize.X, ClientSize.Y);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0.0, ClientSize.X, 0.0, ClientSize.Y, -1.0, 1.0);
            
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            
            GL.Disable(EnableCap.DepthTest);
            
            _renderer.DrawBoard(_board, _currentShape);
            _renderer.DrawUi(_level, _linesCleared, _linesNeeded, _score, _scoreStr, 
                _nextShapeType, _nextShapeColor, _isGameOver, _isPaused);
            
            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            
            var keyboard = KeyboardState;
            _inputHandler.Update(keyboard);
            
            HandleSystemInput(keyboard);
            
            if (_isGameOver || _isPaused)
            {
                return;
            }
            
            HandleMovementInput();
            HandleFastDrop(e);
            HandleAutoFall(e);
        }
        
        private void HandleMovementInput()
        {
            if (_inputHandler.IsMoveLeftPressed())
            {
                _currentShape.MoveLeft(_board);
            }
            
            if (_inputHandler.IsMoveRightPressed())
            {
                _currentShape.MoveRight(_board);
            }
            
            if (_inputHandler.IsRotatePressed())
            {
                _currentShape.Rotate(_board);
            }
        }
        
        private void HandleFastDrop(FrameEventArgs e)
        {
            if (_inputHandler.IsFastDropHeld())
            {
                _fastDropTimer += e.Time;
                double dropInterval = 1.0 / GameConfig.FastDropSpeed;
                
                while (_fastDropTimer >= dropInterval)
                {
                    _fastDropTimer -= dropInterval;
                    bool landed = _currentShape.FastDrop(_board);
                    if (landed)
                    {
                        HandleShapeLanded();
                        _fastDropTimer = 0;
                        break;
                    }
                }
            }
            else
            {
                _fastDropTimer = 0;
            }
        }
        
        private void HandleSystemInput(KeyboardState keyboard)
        {
            if (keyboard.IsKeyDown(Keys.Escape))
            {
                Close();
            }
            
            if (_inputHandler.IsRestartPressed())
            {
                RestartGame();
            }
            
            if (_inputHandler.IsPausePressed() && !_isGameOver)
            {
                _isPaused = !_isPaused;
            }
        }
        
        private void HandleAutoFall(FrameEventArgs e)
        {
            _fallTimer += e.Time;
            
            if (_fallTimer < _currentFallInterval)
            {
                return;
            }
            
            _fallTimer = 0;
            
            if (_currentShape.MoveDown(_board))
            {
                HandleShapeLanded();
            }
        }
        
        private void HandleShapeLanded()
        {
            int lines = _board.RemoveCompleteLines();
    
            if (lines > 0 && lines < GameConfig.LineScores.Length)
            {
                _score += GameConfig.LineScores[lines];
                _linesCleared += lines;
                
                if (_linesCleared >= _linesNeeded)
                {
                    HandleLevelUp();
                }
            }
    
            UpdateScoreString();
            
            _currentShape.ApplyNext(_nextShapeType, _nextShapeColor);
            GenerateNextPiece();
    
            if (_currentShape.CheckCollisionWithBoard(_board))
            {
                _isGameOver = true;
            }
        }
        
        private void HandleLevelUp()
        {
            int emptyLines = CountEmptyLines();
            _score += emptyLines * GameConfig.EmptyLineBonus;
            
            _level++;
            _linesCleared = 0;
            _board.Clear();
            
            InitializeLevel();
            UpdateScoreString();
        }
        
        private int CountEmptyLines()
        {
            int count = 0;
            for (int y = 0; y < Board.Height; y++)
            {
                bool empty = true;
                for (int x = 0; x < Board.Width; x++)
                {
                    if (_board[x, y] >= 2)
                    {
                        empty = false;
                        break;
                    }
                }
                if (empty)
                {
                    count++;
                }
            }
            return count;
        }
        
        private void UpdateScoreString()
        {
            if (_score != _oldScore)
            {
                _oldScore = _score;
                _scoreStr = _score.ToString();
            }
        }
                
        private void RestartGame()
        {
            _isGameOver = false;
            _isPaused = false;
            _level = 1;
            _score = 0;
            _scoreStr = "0";
            _linesCleared = 0;
            _board.Clear();
            InitializeLevel();
            GenerateNextPiece();
            _currentShape.SpawnNew();
            _fallTimer = 0;
            _fastDropTimer = 0;
        }
    }
}
