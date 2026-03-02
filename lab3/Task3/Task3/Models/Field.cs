using Task3.Enums;

namespace Task3.Models
{


    public class Field
    {
        public const int Width = 10;
        public const int Height = 20;
        
        private readonly ColorType[,] _cells = new ColorType[Width, Height];
        
        public ColorType this[int x, int y]
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
                    _cells[i, j] = ColorType.Empty;
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
            
            return _cells[x, y] != ColorType.Empty;
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
            for (int x = 0; x < Width; x++)
            {
                if (_cells[x, lineNumber] == ColorType.Empty)
                {
                    return false;
                }
            }
            
            return true;
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
                _cells[x, 0] = ColorType.Empty;
            }
        }
    }
}
