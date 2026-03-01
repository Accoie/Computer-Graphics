namespace Task3.Configs
{
    public static class GameConfig
    {
        public const int WindowWidth = 800;
        public const int WindowHeight = 600;
        public const string WindowTitle = "Tetris";
        
        public const int CellSize = 24;
        
        public const double FallInterval = 1.0;
        public const double FastDropSpeed = 30.0;
        public const int PointsPerLine = 100;
        
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
