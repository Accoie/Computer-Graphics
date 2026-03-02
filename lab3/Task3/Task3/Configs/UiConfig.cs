namespace Task3.Configs
{
    public static class UiConfig
    {
        public const float TitleScale = 2.5f;
        public const float HeaderScale = 1.5f;

        public const string GameTitle = "TETRIS";
        public const string LevelLabel = "LEVEL";
        public const string LinesLabel = "LINES";
        public const string ScoreLabel = "SCORE";
        public const string NextLabel = "NEXT";
        public const string ControlsHeader = "CONTROLS";
        public const string GameOverText = "GAME OVER";
        public const string FinalScoreText = "FINAL SCORE: ";
        public const string RestartPrompt = "PRESS R TO RESTART";
        public const string PauseText = "PAUSED";
        
        public static readonly string[] ControlInstructions = [
            "LEFT/RIGHT - MOVE",
            "UP - ROTATE", 
            "DOWN - FAST DROP",
            "P - PAUSE",
            "R - RESTART",
            "ESC - EXIT"
        ];
    }
}
