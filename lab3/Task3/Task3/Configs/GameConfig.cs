namespace Task3.Configs
{
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
        
        public static readonly int[] LineScores = { 0, 10, 30, 70, 150 };
        
        public const int BaseLinesPerLevel = 5;
        public const int LinesPerLevelIncrease = 2;
        public const int EmptyLineBonus = 10;
        
        public static readonly float[,] BlockColors = new[,]
        {
            {0.2f, 0.2f, 0.2f},
            {0.8f, 0.8f, 0.8f},
            {1.0f, 0.0f, 0.0f},
            {1.0f, 1.0f, 0.0f},
            {0.0f, 0.5f, 1.0f},
            {0.0f, 0.8f, 0.4f},
            {1.0f, 0.5f, 0.0f},
            {1.0f, 0.0f, 1.0f}
        };
        
        public const float GameOverOverlayAlpha = 0.8f;
    }
}
