namespace Task3.Configs
{
    public static class UiConfig
    {
        public const float TitleScale = 2.5f;
        public const float HeaderScale = 1.5f;
        
        public const string GameTitle = "TETRIS";
        public const string ScoreLabel = "SCORE";
        public const string ControlsHeader = "CONTROLS";
        public const string GameOverText = "GAME OVER";
        public const string FinalScoreText = "FINAL SCORE: ";
        public const string RestartPrompt = "PRESS R TO RESTART";
        
        public static readonly string[] ControlInstructions = [
            "LEFT/RIGHT - MOVE",
            "UP - ROTATE", 
            "SPACE - FAST DROP",
            "R - RESTART",
            "ESC - EXIT"
        ];
    }
}
