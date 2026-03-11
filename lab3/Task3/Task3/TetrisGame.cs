using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL;
using Task3.Configs;
using Task3.Enums;
using Task3.Input;
using Task3.Models;
using Task3.Rendering;

namespace Task3
{
    public class TetrisGame : GameWindow
    {
        private readonly Field _field;
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

        private Shape _nextShape = null!;

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
            _field = new Field();
            _currentShape = new Shape();
            _inputHandler = new InputHandler();
            _renderer = new Renderer(ClientSize.X, ClientSize.Y);

            InitializeLevel();
            GenerateNextShape();
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

        private void GenerateNextShape()
        {
            _nextShape = Shape.GenerateNextShape();
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            Renderer.Initialize();
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

            _renderer.DrawField(_field, _currentShape);
            _renderer.DrawUi(CreateUiState());

            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            _inputHandler.Update(KeyboardState);

            HandleSystemInput(KeyboardState);

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
                _currentShape.MoveLeft(_field);
            }

            if (_inputHandler.IsMoveRightPressed())
            {
                _currentShape.MoveRight(_field);
            }

            if (_inputHandler.IsRotatePressed())
            {
                _currentShape.Rotate(_field);
            }
        }

        private void HandleFastDrop(FrameEventArgs e)
        {
            if (_inputHandler.IsFastDropHeld())
            {
                _fastDropTimer += e.Time;
                const double dropInterval = 1.0 / GameConfig.FastDropSpeed;

                while (_fastDropTimer >= dropInterval)
                {
                    _fastDropTimer -= dropInterval;
                    bool canMoveDown = _currentShape.TryMoveDown(_field);
                    if (!canMoveDown)
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

            if (!_currentShape.TryMoveDown(_field))
            {
                HandleShapeLanded();
            }
        }

        private void HandleShapeLanded()
        {
            int lines = _field.RemoveCompleteLines();

            if (lines > 0)
            {
                int scoreIndex = Math.Min(lines, GameConfig.LineScores.Length - 1);
                _score += GameConfig.LineScores[scoreIndex];
                _linesCleared += lines;

                if (_linesCleared >= _linesNeeded)
                {
                    HandleLevelUp();
                }
            }

            UpdateScoreString();

            if (_isGameOver)
            {
                return;
            }
            
            _currentShape.ApplyNext(_nextShape);
            GenerateNextShape();

            if (_currentShape.CheckCollisionWithField(_field))
            {
                _isGameOver = true;
            }
        }

        private void HandleLevelUp()
        {
            _score += CalculateEmptyLineBonus();

            _level++;
            _linesCleared = 0;
            _field.Clear();

            InitializeLevel();
        }

        private int CalculateEmptyLineBonus()
        {
            int emptyLines = 0;
            for (int y = 0; y < Field.Height; y++)
            {
                bool empty = true;
                for (int x = 0; x < Field.Width; x++)
                {
                    if (_field[x, y] >= ColorType.Red)
                    {
                        empty = false;
                        break;
                    }
                }

                if (empty)
                {
                    emptyLines++;
                }
            }

            return emptyLines * GameConfig.EmptyLineBonus;
        }

        private void UpdateScoreString()
        {
            if (_score != _oldScore)
            {
                _oldScore = _score;
                _scoreStr = _score.ToString();
            }
        }

        private UiState CreateUiState()
        {
            return new UiState
            {
                Level = _level,
                LinesCleared = _linesCleared,
                LinesNeeded = _linesNeeded,
                Score = _score,
                ScoreString = _scoreStr,
                NextShape = _nextShape,
                IsGameOver = _isGameOver,
                IsPaused = _isPaused
            };
        }

        private void RestartGame()
        {
            _isGameOver = false;
            _isPaused = false;
            _level = 1;
            _score = 0;
            _scoreStr = "0";
            _linesCleared = 0;
            _field.Clear();
            InitializeLevel();
            GenerateNextShape();
            _currentShape.SpawnNew();
            _fallTimer = 0;
            _fastDropTimer = 0;
        }
    }
}