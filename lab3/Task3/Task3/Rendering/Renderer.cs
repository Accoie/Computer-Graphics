using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using Task3.Configs;
using Task3.Enums;
using Task3.Models;

namespace Task3.Rendering
{
    public class Renderer
    {
        private const int CellSize = GameConfig.CellSize;
        private int _fieldOffsetX;
        private int _fieldOffsetY;
        private int _windowWidth;
        private int _windowHeight;
        
        private readonly Dictionary<ColorType, Color4> _colors = GameConfig.CellColors;
        private readonly FontRenderer _fontRenderer = new();

        public Renderer(int width, int height)
        {
            UpdateWindowSize(width, height);
        }

        public void UpdateWindowSize(int width, int height)
        {
            _windowWidth = width;
            _windowHeight = height;
            _fieldOffsetX = (width - Field.Width * CellSize) / 2;
            _fieldOffsetY = (height - Field.Height * CellSize) / 2;
        }
        
        public void Initialize()
        {
            GL.ClearColor(0.15f, 0.10f, 0.1f, 1.0f);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        }
        
        public void DrawField(Field field, Shape currentShape)
        {
            int fieldWidth = Field.Width * CellSize;
            int fieldHeight = Field.Height * CellSize;
            
            DrawFieldBackground(fieldWidth, fieldHeight);
            DrawPlacedCells(field);
            DrawCurrentShape(currentShape);
            DrawFieldBorder(fieldWidth, fieldHeight);
            DrawGridLines(fieldWidth, fieldHeight);
        }
        
        private void DrawFieldBackground(int fieldWidth, int fieldHeight)
        {
            GL.Color4(0.1f, 0.05f, 0.05f, 1.0f);
            GL.Begin(PrimitiveType.Quads);
            GL.Vertex2(_fieldOffsetX, _fieldOffsetY);
            GL.Vertex2(_fieldOffsetX + fieldWidth, _fieldOffsetY);
            GL.Vertex2(_fieldOffsetX + fieldWidth, _fieldOffsetY + fieldHeight);
            GL.Vertex2(_fieldOffsetX, _fieldOffsetY + fieldHeight);
            GL.End();
        }
        
        private void DrawPlacedCells(Field field)
        {
            for (int x = 0; x < Field.Width; x++)
            {
                for (int y = 0; y < Field.Height; y++)
                {
                    ColorType colorType = field[x, y];
                    if (colorType >= ColorType.Red)
                    {
                        DrawCell(x, y, colorType);
                    }
                }
            }
        }
        
        private void DrawCurrentShape(Shape currentShape)
        {
            for (int i = 0; i < 4; i++)
            {
                Block block = currentShape[i];
                if (block.X is >= 0 and < Field.Width && block.Y < Field.Height)
                {
                    DrawCell(block.X, block.Y, currentShape.Color);
                }
            }
        }
        
        private void DrawCell(int x, int y, ColorType colorType)
        {
            float drawX = _fieldOffsetX + x * CellSize;
            float drawY = _fieldOffsetY + (Field.Height - 1 - y) * CellSize;
            DrawSquare(drawX, drawY, CellSize - 1, colorType);
        }
        
        private void DrawFieldBorder(int fieldWidth, int fieldHeight)
        {
            GL.Color4(1.0f, 1.0f, 1.0f, 1.0f);
            GL.LineWidth(2.0f);
            GL.Begin(PrimitiveType.LineLoop);
            GL.Vertex2(_fieldOffsetX, _fieldOffsetY);
            GL.Vertex2(_fieldOffsetX + fieldWidth, _fieldOffsetY);
            GL.Vertex2(_fieldOffsetX + fieldWidth, _fieldOffsetY + fieldHeight);
            GL.Vertex2(_fieldOffsetX, _fieldOffsetY + fieldHeight);
            GL.End();
            GL.LineWidth(1.0f);
        }
        
        private void DrawGridLines(int fieldWidth, int fieldHeight)
        {
            GL.Color4(0.3f, 0.3f, 0.4f, 1.0f);
            DrawVerticalGridLines(fieldHeight);
            DrawHorizontalGridLines(fieldWidth);
        }
        
        private void DrawVerticalGridLines(int fieldHeight)
        {
            for (int i = 1; i < Field.Width; i++)
            {
                float lineX = _fieldOffsetX + i * CellSize;
                GL.Begin(PrimitiveType.Lines);
                GL.Vertex2(lineX, _fieldOffsetY);
                GL.Vertex2(lineX, _fieldOffsetY + fieldHeight);
                GL.End();
            }
        }
        
        private void DrawHorizontalGridLines(int fieldWidth)
        {
            for (int i = 1; i < Field.Height; i++)
            {
                float lineY = _fieldOffsetY + i * CellSize;
                GL.Begin(PrimitiveType.Lines);
                GL.Vertex2(_fieldOffsetX, lineY);
                GL.Vertex2(_fieldOffsetX + fieldWidth, lineY);
                GL.End();
            }
        }
        
        public void DrawUi(UiState uiState)
        {
            int fieldWidth = Field.Width * CellSize;
            int fieldHeight = Field.Height * CellSize;
            
            DrawTitle(fieldWidth, fieldHeight);
            DrawInfoPanel(uiState.Level, uiState.LinesCleared, uiState.LinesNeeded, uiState.ScoreString, fieldWidth, fieldHeight);
            
            if (uiState.NextShape is not null)
            {
                DrawNextPiecePreview(uiState.NextShape, fieldWidth, fieldHeight);
            }
            
            DrawControls(fieldHeight);
            
            if (uiState.IsGameOver)
            {
                DrawGameOverOverlay(uiState.Score, fieldWidth, fieldHeight);
            }
            else if (uiState.IsPaused)
            {
                DrawPauseOverlay(fieldWidth, fieldHeight);
            }
        }
        
        private void DrawTitle(int fieldWidth, int fieldHeight)
        {
            float titleY = _fieldOffsetY + fieldHeight + 30;
            _fontRenderer.DrawTextCenteredOnArea(_fieldOffsetX, fieldWidth, titleY, UiConfig.GameTitle, UiConfig.TitleScale);
        }
        
        private void DrawInfoPanel(int level, int linesCleared, int linesNeeded, string scoreStr,
            int fieldWidth, int fieldHeight)
        {
            float infoX = _fieldOffsetX + fieldWidth + 30;
            float infoY = _fieldOffsetY + fieldHeight - 30;
            
            _fontRenderer.DrawText(infoX, infoY, UiConfig.LevelLabel, UiConfig.HeaderScale);
            _fontRenderer.DrawText(infoX, infoY - 35, level.ToString(), UiConfig.HeaderScale);
            
            infoY -= 90;
            _fontRenderer.DrawText(infoX, infoY, UiConfig.LinesLabel, UiConfig.HeaderScale);
            _fontRenderer.DrawText(infoX, infoY - 35, $"{linesCleared}/{linesNeeded}", UiConfig.HeaderScale);
            
            infoY -= 90;
            _fontRenderer.DrawText(infoX, infoY, UiConfig.ScoreLabel, UiConfig.HeaderScale);
            _fontRenderer.DrawText(infoX, infoY - 35, scoreStr, UiConfig.HeaderScale);
        }
        
        private void DrawNextPiecePreview(Shape nextShape, int fieldWidth, int fieldHeight)
        {
            float previewX = _fieldOffsetX + fieldWidth + 30;
            float previewY = _fieldOffsetY + fieldHeight - 320;
            
            _fontRenderer.DrawText(previewX, previewY, UiConfig.NextLabel, UiConfig.HeaderScale);
            
            float offsetX = previewX - 70;
            float offsetY = previewY - 60;
            
            DrawPreviewBlocks(nextShape, offsetX, offsetY);
        }
        
        private void DrawPreviewBlocks(Shape shape, float offsetX, float offsetY)
        {
            int сellSize = 20;
            
            for (int i = 0; i < 4; i++)
            {
                float x = offsetX + shape[i].X * сellSize;
                float y = offsetY - shape[i].Y * сellSize;
                DrawPreviewCell(x, y, сellSize, shape.Color);
            }
        }
        
        private void DrawPreviewCell(float x, float y, int size, ColorType colorType)
        {
            Color4 color = _colors.GetValueOrDefault(colorType, _colors[ColorType.Empty]);
            
            GL.Color4(color);
            GL.Begin(PrimitiveType.Quads);
            GL.Vertex2(x, y);
            GL.Vertex2(x + size, y);
            GL.Vertex2(x + size, y + size);
            GL.Vertex2(x, y + size);
            GL.End();
        }
        
        private void DrawControls(int fieldHeight)
        {
            float controlsX = _fieldOffsetX - 220;
            float controlsY = _fieldOffsetY + fieldHeight - 30;
            
            _fontRenderer.DrawText(controlsX, controlsY, UiConfig.ControlsHeader, UiConfig.HeaderScale);
            controlsY -= 40;
            
            foreach (string instruction in UiConfig.ControlInstructions)
            {
                _fontRenderer.DrawText(controlsX, controlsY, instruction);
                controlsY -= 25;
            }
        }
        
        private void DrawGameOverOverlay(int score, int fieldWidth, int fieldHeight)
        {
            GL.Color4(0.0f, 0.0f, 0.0f, GameConfig.GameOverOverlayAlpha);
            GL.Begin(PrimitiveType.Quads);
            GL.Vertex2(0, 0);
            GL.Vertex2(_windowWidth, 0);
            GL.Vertex2(_windowWidth, _windowHeight);
            GL.Vertex2(0, _windowHeight);
            GL.End();
            
            float fieldCenterY = _fieldOffsetY + fieldHeight / 2.0f;
            _fontRenderer.DrawTextCenteredOnArea(_fieldOffsetX, fieldWidth, fieldCenterY + 60, UiConfig.GameOverText, UiConfig.TitleScale);
            _fontRenderer.DrawTextCenteredOnArea(_fieldOffsetX, fieldWidth, fieldCenterY, $"{UiConfig.FinalScoreText}{score}", UiConfig.HeaderScale);
            _fontRenderer.DrawTextCenteredOnArea(_fieldOffsetX, fieldWidth, fieldCenterY - 50, UiConfig.RestartPrompt, UiConfig.HeaderScale);
        }
        
        private void DrawPauseOverlay(int fieldWidth, int fieldHeight)
        {
            GL.Color4(0.0f, 0.0f, 0.0f, 0.6f);
            GL.Begin(PrimitiveType.Quads);
            GL.Vertex2(0, 0);
            GL.Vertex2(_windowWidth, 0);
            GL.Vertex2(_windowWidth, _windowHeight);
            GL.Vertex2(0, _windowHeight);
            GL.End();
            
            float fieldCenterY = _fieldOffsetY + fieldHeight / 2.0f;
            _fontRenderer.DrawTextCenteredOnArea(_fieldOffsetX, fieldWidth, fieldCenterY, UiConfig.PauseText, UiConfig.TitleScale);
        }
        
        private void DrawSquare(float x, float y, float size, ColorType colorType)
        {
            Color4 color = _colors.GetValueOrDefault(colorType, _colors[ColorType.Empty]);
            
            GL.Color4(color);
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
