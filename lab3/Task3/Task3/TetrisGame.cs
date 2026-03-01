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
        
        private bool _isGameOver;
        
        private double _fallTimer;
        private double _fastDropTimer;
        private const double FallInterval = GameConfig.FallInterval;

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
            
            _currentShape.SpawnNew(_board);
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
            _renderer.DrawUi(_score, _scoreStr, _isGameOver);
            
            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            
            var keyboard = KeyboardState;
            _inputHandler.Update(keyboard);
            
            HandleMovementInput();
            HandleFastDrop(e);
            HandleSystemInput(keyboard);
            HandleAutoFall(e);
        }
        
        private void HandleMovementInput()
        {
            if (_isGameOver)
            {
                return;
            }
            
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
            if (_isGameOver)
            {
                return;
            }
            
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
        }
        
        private void HandleAutoFall(FrameEventArgs e)
        {
            _fallTimer += e.Time;
            
            if (_fallTimer < FallInterval || _isGameOver)
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
            _score += lines * GameConfig.PointsPerLine;
            UpdateScoreString();
            
            _currentShape.SpawnNew(_board);
            
            if (_currentShape.CheckCollisionWithBoard(_board))
            {
                _isGameOver = true;
            }
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
            _score = 0;
            _scoreStr = "0";
            _board.Clear();
            _currentShape.SpawnNew(_board);
            _fallTimer = 0;
            _fastDropTimer = 0;
        }

    }
}
