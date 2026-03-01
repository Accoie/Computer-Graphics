namespace Task3.Models
{
    public class Board
    {
        public const int Width = 10;
        public const int Height = 20;
        
        private readonly int[,] _cells = new int[Width, Height];
        
        public int this[int x, int y]
        {
            get => _cells[x, y];
            set => _cells[x, y] = value;
        }
        
        public void Clear()
        {
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    _cells[i, j] = 0;
                }
            }
        }

        public bool IsOccupied(int x, int y)
        {
            if (x < 0 || x >= Width || y >= Height)
            {
                return true;
            }
            if (y < 0)
            {
                return false;
            }
            
            return _cells[x, y] >= 2;
        }
        
        public int RemoveCompleteLines()
        {
            int linesRemoved = 0;
            
            for (int y = 0; y < Height; y++)
            {
                bool isLineComplete = CheckLineCompletion(y);
                
                if (isLineComplete)
                {
                    ShiftLinesDown(y);
                    ClearTopLine();
                    linesRemoved++;
                    y--;
                }
            }
            
            return linesRemoved;
        }

        private bool CheckLineCompletion(int lineNumber)
        {
            bool complete = true;
            for (int x = 0; x < Width; x++)
            {
                if (_cells[x, lineNumber] < 2)
                {
                    complete = false;
                    
                    break;
                }
            }
            
            return complete;
        }

        private void ShiftLinesDown(int startLine)
        {
            for (int y = startLine; y > 0; y--)
            {
                for (int x = 0; x < Width; x++)
                {
                    _cells[x, y] = _cells[x, y - 1];
                }
            }
        }

        private void ClearTopLine()
        {
            for (int x = 0; x < Width; x++)
            {
                _cells[x, 0] = 0;
            }
        }
    }
}
