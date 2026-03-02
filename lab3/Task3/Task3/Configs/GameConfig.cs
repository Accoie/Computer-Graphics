using Task3.Enums;

namespace Task3.Configs
{
    using Task3.Models;
    using OpenTK.Mathematics;

    public static class GameConfig
    {
        public const int WindowWidth = 800;
        public const int WindowHeight = 600;
        public const string WindowTitle = "Tetris Game - OpenTK 4.x";
        
        public const int CellSize = 24;
        
        public const double BaseFallInterval = 1.0;
        public const double FallIntervalDecrease = 0.1;
        public const double MinFallInterval = 0.1;
        public const double FastDropSpeed = 30.0;
        
        public static readonly int[] LineScores = [0, 10, 30, 70, 150];
        
        public const int BaseLinesPerLevel = 5;
        public const int LinesPerLevelIncrease = 2;
        public const int EmptyLineBonus = 10;
        
        public static readonly Dictionary<ColorType, Color4> CellColors = new()
        {
            { ColorType.Empty, new Color4(0.2f, 0.2f, 0.2f, 1.0f) },
            { ColorType.Red, new Color4(1.0f, 0.0f, 0.0f, 1.0f) },
            { ColorType.Yellow, new Color4(1.0f, 1.0f, 0.0f, 1.0f) },
            { ColorType.Blue, new Color4(0.0f, 0.5f, 1.0f, 1.0f) },
            { ColorType.Green, new Color4(0.0f, 0.8f, 0.4f, 1.0f) },
            { ColorType.Orange, new Color4(1.0f, 0.5f, 0.0f, 1.0f) },
            { ColorType.Purple, new Color4(1.0f, 0.0f, 1.0f, 1.0f) }
        };
        
        public const float GameOverOverlayAlpha = 0.8f;
    }
}
