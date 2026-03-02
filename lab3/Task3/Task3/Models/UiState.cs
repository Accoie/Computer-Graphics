namespace Task3.Models
{
    public class UiState
    {
        public int Level { get; init; }
        public int LinesCleared { get; init; }
        public int LinesNeeded { get; init; }
        public int Score { get; init; }
        public string ScoreString { get; init; } = "0";
        public Shape? NextShape { get; init; }
        public bool IsGameOver { get; init; }
        public bool IsPaused { get; init; }
    }
}
