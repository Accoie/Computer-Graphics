using OpenTK.Graphics.OpenGL;
using Task3.Configs;

namespace Task3
{
    public class Renderer
    {
        private readonly int _cellSize = GameConfig.CellSize;
        private int _boardOffsetX;
        private int _boardOffsetY;
        private int _windowWidth;
        private int _windowHeight;
        
        private readonly float[,] _colors = GameConfig.BlockColors;
        private readonly FontRenderer _fontRenderer = new();

        public Renderer(int width, int height)
        {
            UpdateWindowSize(width, height);
        }

        public void UpdateWindowSize(int width, int height)
        {
            _windowWidth = width;
            _windowHeight = height;
            _boardOffsetX = (width - Board.Width * _cellSize) / 2;
            _boardOffsetY = (height - Board.Height * _cellSize) / 2;
        }
        
        public void Initialize()
        {
            GL.ClearColor(0.05f, 0.05f, 0.1f, 1.0f);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        }
        
        public void DrawBoard(Board board, Shape currentShape)
        {
            int boardWidth = Board.Width * _cellSize;
            int boardHeight = Board.Height * _cellSize;
            
            DrawBoardBackground(boardWidth, boardHeight);
            DrawPlacedCells(board);
            DrawCurrentShape(currentShape);
            DrawBoardBorder(boardWidth, boardHeight);
            DrawGridLines(boardWidth, boardHeight);
        }
        
        private void DrawBoardBackground(int boardWidth, int boardHeight)
        {
            GL.Color4(0.1f, 0.1f, 0.15f, 1.0f);
            GL.Begin(PrimitiveType.Quads);
            GL.Vertex2(_boardOffsetX, _boardOffsetY);
            GL.Vertex2(_boardOffsetX + boardWidth, _boardOffsetY);
            GL.Vertex2(_boardOffsetX + boardWidth, _boardOffsetY + boardHeight);
            GL.Vertex2(_boardOffsetX, _boardOffsetY + boardHeight);
            GL.End();
        }
        
        private void DrawPlacedCells(Board board)
        {
            for (int x = 0; x < Board.Width; x++)
            {
                for (int y = 0; y < Board.Height; y++)
                {
                    int colorIndex = board[x, y];
                    if (colorIndex >= 2)
                    {
                        DrawCell(x, y, colorIndex);
                    }
                }
            }
        }
        
        private void DrawCurrentShape(Shape currentShape)
        {
            for (int i = 0; i < 4; i++)
            {
                int x = currentShape[i, 0];
                int y = currentShape[i, 1];
                if (x >= 0 && x < Board.Width && y < Board.Height)
                {
                    DrawCell(x, y, currentShape.Color);
                }
            }
        }
        
        private void DrawCell(int x, int y, int colorIndex)
        {
            float drawX = _boardOffsetX + x * _cellSize;
            float drawY = _boardOffsetY + (Board.Height - 1 - y) * _cellSize;
            DrawSquare(drawX, drawY, _cellSize - 1, colorIndex);
        }
        
        private void DrawBoardBorder(int boardWidth, int boardHeight)
        {
            GL.Color4(1.0f, 1.0f, 1.0f, 1.0f);
            GL.LineWidth(2.0f);
            GL.Begin(PrimitiveType.LineLoop);
            GL.Vertex2(_boardOffsetX, _boardOffsetY);
            GL.Vertex2(_boardOffsetX + boardWidth, _boardOffsetY);
            GL.Vertex2(_boardOffsetX + boardWidth, _boardOffsetY + boardHeight);
            GL.Vertex2(_boardOffsetX, _boardOffsetY + boardHeight);
            GL.End();
            GL.LineWidth(1.0f);
        }
        
        private void DrawGridLines(int boardWidth, int boardHeight)
        {
            GL.Color4(0.3f, 0.3f, 0.4f, 1.0f);
            DrawVerticalGridLines(boardHeight);
            DrawHorizontalGridLines(boardWidth);
        }
        
        private void DrawVerticalGridLines(int boardHeight)
        {
            for (int i = 1; i < Board.Width; i++)
            {
                float lineX = _boardOffsetX + i * _cellSize;
                GL.Begin(PrimitiveType.Lines);
                GL.Vertex2(lineX, _boardOffsetY);
                GL.Vertex2(lineX, _boardOffsetY + boardHeight);
                GL.End();
            }
        }
        
        private void DrawHorizontalGridLines(int boardWidth)
        {
            for (int i = 1; i < Board.Height; i++)
            {
                float lineY = _boardOffsetY + i * _cellSize;
                GL.Begin(PrimitiveType.Lines);
                GL.Vertex2(_boardOffsetX, lineY);
                GL.Vertex2(_boardOffsetX + boardWidth, lineY);
                GL.End();
            }
        }
        
        public void DrawUi(int level, int linesCleared, int linesNeeded, int score, string scoreStr,
            ShapeType nextShapeType, int nextShapeColor, bool isGameOver, bool isPaused)
        {
            int boardWidth = Board.Width * _cellSize;
            int boardHeight = Board.Height * _cellSize;
            
            DrawTitle(boardWidth, boardHeight);
            DrawInfoPanel(level, linesCleared, linesNeeded, score, scoreStr, boardWidth, boardHeight);
            DrawNextPiecePreview(nextShapeType, nextShapeColor, boardWidth, boardHeight);
            DrawControls(boardHeight);
            
            if (isGameOver)
            {
                DrawGameOverOverlay(score, boardWidth, boardHeight);
            }
            else if (isPaused)
            {
                DrawPauseOverlay(boardWidth, boardHeight);
            }
        }
        
        private void DrawTitle(int boardWidth, int boardHeight)
        {
            float titleY = _boardOffsetY + boardHeight + 30;
            _fontRenderer.DrawTextCenteredOnArea(_boardOffsetX, boardWidth, titleY, UiConfig.GameTitle, UiConfig.TitleScale);
        }
        
        private void DrawInfoPanel(int level, int linesCleared, int linesNeeded, int score, string scoreStr,
            int boardWidth, int boardHeight)
        {
            float infoX = _boardOffsetX + boardWidth + 30;
            float infoY = _boardOffsetY + boardHeight - 30;
            
            _fontRenderer.DrawText(infoX, infoY, UiConfig.LevelLabel, UiConfig.HeaderScale);
            _fontRenderer.DrawText(infoX, infoY - 35, level.ToString(), UiConfig.HeaderScale);
            
            infoY -= 90;
            _fontRenderer.DrawText(infoX, infoY, UiConfig.LinesLabel, UiConfig.HeaderScale);
            _fontRenderer.DrawText(infoX, infoY - 35, $"{linesCleared}/{linesNeeded}", UiConfig.HeaderScale);
            
            infoY -= 90;
            _fontRenderer.DrawText(infoX, infoY, UiConfig.ScoreLabel, UiConfig.HeaderScale);
            _fontRenderer.DrawText(infoX, infoY - 35, scoreStr, UiConfig.HeaderScale);
        }
        
        private void DrawNextPiecePreview(ShapeType nextShapeType, int nextShapeColor, int boardWidth, int boardHeight)
        {
            float previewX = _boardOffsetX + boardWidth + 30;
            float previewY = _boardOffsetY + boardHeight - 320;
            
            _fontRenderer.DrawText(previewX, previewY, UiConfig.NextLabel, UiConfig.HeaderScale);
            
            int[,] blocks = Shape.GetPreviewBlocks(nextShapeType);
            int previewCellSize = 20;
            float offsetX = previewX;
            float offsetY = previewY - 60;
            
            DrawPreviewBlocks(blocks, offsetX, offsetY, previewCellSize, nextShapeColor);
        }
        
        private void DrawPreviewBlocks(int[,] blocks, float offsetX, float offsetY, int cellSize, int colorIndex)
        {
            for (int i = 0; i < 4; i++)
            {
                float x = offsetX + blocks[i, 0] * cellSize;
                float y = offsetY - blocks[i, 1] * cellSize;
                DrawPreviewCell(x, y, cellSize, colorIndex);
            }
        }
        
        private void DrawPreviewCell(float x, float y, int size, int colorIndex)
        {
            if (colorIndex < 0 || colorIndex >= 8)
            {
                colorIndex = 0;
            }
            
            float r = _colors[colorIndex, 0];
            float g = _colors[colorIndex, 1];
            float b = _colors[colorIndex, 2];
            
            GL.Color4(r, g, b, 1.0f);
            GL.Begin(PrimitiveType.Quads);
            GL.Vertex2(x, y);
            GL.Vertex2(x + size, y);
            GL.Vertex2(x + size, y + size);
            GL.Vertex2(x, y + size);
            GL.End();
        }
        
        private void DrawControls(int boardHeight)
        {
            float controlsX = _boardOffsetX - 250;
            float controlsY = _boardOffsetY + boardHeight - 30;
            _fontRenderer.DrawText(controlsX, controlsY, UiConfig.ControlsHeader, UiConfig.HeaderScale);
            controlsY -= 40;
            foreach (string instruction in UiConfig.ControlInstructions)
            {
                _fontRenderer.DrawText(controlsX, controlsY, instruction, UiConfig.SmallScale);
                controlsY -= 25;
            }
        }
        
        private void DrawGameOverOverlay(int score, int boardWidth, int boardHeight)
        {
            GL.Color4(0.0f, 0.0f, 0.0f, GameConfig.GameOverOverlayAlpha);
            GL.Begin(PrimitiveType.Quads);
            GL.Vertex2(0, 0);
            GL.Vertex2(_windowWidth, 0);
            GL.Vertex2(_windowWidth, _windowHeight);
            GL.Vertex2(0, _windowHeight);
            GL.End();
            
            float boardCenterY = _boardOffsetY + boardHeight / 2.0f;
            _fontRenderer.DrawTextCenteredOnArea(_boardOffsetX, boardWidth, boardCenterY + 60, UiConfig.GameOverText, UiConfig.TitleScale);
            _fontRenderer.DrawTextCenteredOnArea(_boardOffsetX, boardWidth, boardCenterY, $"{UiConfig.FinalScoreText}{score}", UiConfig.HeaderScale);
            _fontRenderer.DrawTextCenteredOnArea(_boardOffsetX, boardWidth, boardCenterY - 50, UiConfig.RestartPrompt, UiConfig.HeaderScale);
        }
        
        private void DrawPauseOverlay(int boardWidth, int boardHeight)
        {
            GL.Color4(0.0f, 0.0f, 0.0f, 0.6f);
            GL.Begin(PrimitiveType.Quads);
            GL.Vertex2(0, 0);
            GL.Vertex2(_windowWidth, 0);
            GL.Vertex2(_windowWidth, _windowHeight);
            GL.Vertex2(0, _windowHeight);
            GL.End();
            
            float boardCenterY = _boardOffsetY + boardHeight / 2.0f;
            _fontRenderer.DrawTextCenteredOnArea(_boardOffsetX, boardWidth, boardCenterY, UiConfig.PauseText, UiConfig.TitleScale);
        }
        
        private void DrawSquare(float x, float y, float size, int colorIndex)
        {
            if (colorIndex < 0 || colorIndex >= 8)
            {
                colorIndex = 0;
            }
            
            float r = _colors[colorIndex, 0];
            float g = _colors[colorIndex, 1];
            float b = _colors[colorIndex, 2];
            
            GL.Color4(r, g, b, 1.0f);
            GL.Begin(PrimitiveType.Quads);
            GL.Vertex2(x, y);
            GL.Vertex2(x + size, y);
            GL.Vertex2(x + size, y + size);
            GL.Vertex2(x, y + size);
            GL.End();
            
            GL.Color4(0.3f, 0.3f, 0.3f, 1.0f);
            GL.Begin(PrimitiveType.LineLoop);
            GL.Vertex2(x, y);
            GL.Vertex2(x + size, y);
            GL.Vertex2(x + size, y + size);
            GL.Vertex2(x, y + size);
            GL.End();
        }
    }
}
