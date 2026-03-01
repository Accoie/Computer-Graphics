namespace Task3
{
    public enum ShapeType
    {
        Line,
        Square, 
        BlockL,
        ZigZag, 
        BlockT,
    }

    public class Shape
    {
        private readonly int[,] _blocks = new int[4, 2];
        private ShapeType _shapeType;
        private int _color;
        
        private static readonly ShapeType[] AllShapeTypes = Enum.GetValues<ShapeType>();
        private readonly Random _rand = new();
        
        public int this[int blockIndex, int coord] => _blocks[blockIndex, coord];
        public int Color => _color;
        
        public void SpawnNew(Board board)
        {
            _shapeType = AllShapeTypes[_rand.Next(AllShapeTypes.Length)];
            _color = _rand.Next(6) + 2;
            SetInitialBlocks();
        }
        
        public bool CheckCollisionWithBoard(Board board)
        {
            for (int i = 0; i < 4; i++)
            {
                int x = _blocks[i, 0];
                int y = _blocks[i, 1];
                bool insideBoardAndOccupied = y >= 0 && board.IsOccupied(x, y);
                if (insideBoardAndOccupied)
                {
                    return true;
                }
            }
            
            return false;
        }
        
        public bool MoveDown(Board board)
        {
            if (!CanMoveDown(board))
            {
                LockToBoard(board);
                
                return true;
            }
            
            ShiftBlocks(dx: 0, dy: 1);
            
            return false;
        }
        
        public void MoveRight(Board board)
        {
            if (CanMoveHorizontally(board, 1))
            {
                ShiftBlocks(dx: 1, dy: 0);
            }
        }

        public void MoveLeft(Board board)
        {
            if (CanMoveHorizontally(board, -1))
            {
                ShiftBlocks(dx: -1, dy: 0);
            }
        }
        
        public void Rotate(Board board)
        {
            if (_shapeType == ShapeType.Square)
            {
                return;
            }
            
            int[,] temp = CopyBlocks();
            
            if (_shapeType == ShapeType.Line)
            {
                RotateIShape(temp);
            }
            else
            {
                RotateAroundCenter(temp);
            }
            
            TryWallKicks(board, temp);
        }
        
        public bool FastDrop(Board board)
        {
            if (!CanMoveDown(board))
            {
                LockToBoard(board);
                
                return true;
            }
            
            ShiftBlocks(dx: 0, dy: 1);
            
            return false;
        }

        private void SetInitialBlocks()
        {
            switch (_shapeType)
            {
                case ShapeType.Line:
                    SetLineShape();
                    break;
                case ShapeType.Square:
                    SetSquareShape();
                    break;
                case ShapeType.BlockL:
                    SetBlockLShape();
                    break;
                case ShapeType.ZigZag:
                    SetZigZagShape();
                    break;
                case ShapeType.BlockT:
                    SetBlockTShape();
                    break;
            }
        }

        private void SetLineShape()
        {
            _blocks[0, 0] = 3; _blocks[0, 1] = 0;
            _blocks[1, 0] = 4; _blocks[1, 1] = 0;
            _blocks[2, 0] = 5; _blocks[2, 1] = 0;
            _blocks[3, 0] = 6; _blocks[3, 1] = 0;
        }

        private void SetSquareShape()
        {
            _blocks[0, 0] = 4; _blocks[0, 1] = 0;
            _blocks[1, 0] = 5; _blocks[1, 1] = 0;
            _blocks[2, 0] = 4; _blocks[2, 1] = 1;
            _blocks[3, 0] = 5; _blocks[3, 1] = 1;
        }

        private void SetBlockLShape()
        {
            _blocks[0, 0] = 4; _blocks[0, 1] = 0;
            _blocks[1, 0] = 5; _blocks[1, 1] = 0;
            _blocks[2, 0] = 6; _blocks[2, 1] = 0;
            _blocks[3, 0] = 6; _blocks[3, 1] = 1;
        }

        private void SetZigZagShape()
        {
            _blocks[0, 0] = 5; _blocks[0, 1] = 0;
            _blocks[1, 0] = 6; _blocks[1, 1] = 0;
            _blocks[2, 0] = 4; _blocks[2, 1] = 1;
            _blocks[3, 0] = 5; _blocks[3, 1] = 1;
        }

        private void SetBlockTShape()
        {
            _blocks[0, 0] = 4; _blocks[0, 1] = 0;
            _blocks[1, 0] = 5; _blocks[1, 1] = 0;
            _blocks[2, 0] = 6; _blocks[2, 1] = 0;
            _blocks[3, 0] = 5; _blocks[3, 1] = 1;
        }

        private bool CanMoveDown(Board board)
        {
            for (int i = 0; i < 4; i++)
            {
                int x = _blocks[i, 0];
                int newY = _blocks[i, 1] + 1;
                bool hitBottom = newY >= Board.Height;
                bool hitBlock = board.IsOccupied(x, newY);
                
                if (hitBottom || hitBlock)
                {
                    return false;
                }
            }
            return true;
        }

        private bool CanMoveHorizontally(Board board, int dx)
        {
            for (int i = 0; i < 4; i++)
            {
                int newX = _blocks[i, 0] + dx;
                int y = _blocks[i, 1];
                bool outOfBounds = newX < 0 || newX >= Board.Width;
                bool hitBlock = board.IsOccupied(newX, y);
                
                if (outOfBounds || hitBlock)
                {
                    return false;
                }
            }
            return true;
        }

        private void ShiftBlocks(int dx, int dy)
        {
            for (int i = 0; i < 4; i++)
            {
                _blocks[i, 0] += dx;
                _blocks[i, 1] += dy;
            }
        }

        private void LockToBoard(Board board)
        {
            for (int i = 0; i < 4; i++)
            {
                int x = _blocks[i, 0];
                int y = _blocks[i, 1];
                bool insideBoard = x >= 0 && x < Board.Width && y >= 0 && y < Board.Height;
                if (insideBoard)
                {
                    board[x, y] = _color;
                }
            }
        }

        private int[,] CopyBlocks()
        {
            int[,] copy = new int[4, 2];
            for (int i = 0; i < 4; i++)
            {
                copy[i, 0] = _blocks[i, 0];
                copy[i, 1] = _blocks[i, 1];
            }
            return copy;
        }

        private void RotateAroundCenter(int[,] temp)
        {
            int cx = _blocks[1, 0];
            int cy = _blocks[1, 1];
            
            for (int i = 0; i < 4; i++)
            {
                int x = _blocks[i, 0] - cx;
                int y = _blocks[i, 1] - cy;
                temp[i, 0] = cx - y;
                temp[i, 1] = cy + x;
            }
        }

        private void RotateIShape(int[,] temp)
        {
            int cx = _blocks[1, 0];
            int cy = _blocks[1, 1];
            bool isHorizontal = _blocks[0, 1] == _blocks[1, 1];
            
            if (isHorizontal)
            {
                temp[0, 0] = cx;    
                temp[0, 1] = cy + 1;
                
                temp[1, 0] = cx;     
                temp[1, 1] = cy;
                
                temp[2, 0] = cx;    
                temp[2, 1] = cy - 1;
                
                temp[3, 0] = cx;    
                temp[3, 1] = cy - 2;
            }
            else
            {
                temp[0, 0] = cx - 1; 
                temp[0, 1] = cy;
                
                temp[1, 0] = cx;     
                temp[1, 1] = cy;
                
                temp[2, 0] = cx + 1; 
                temp[2, 1] = cy;
                
                temp[3, 0] = cx + 2; 
                temp[3, 1] = cy;
            }
        }

        private void TryWallKicks(Board board, int[,] temp)
        {
            if (TryApplyRotation(board, temp))
            {
                return;
            }
            
            ShiftTempBlocks(temp, dx: -1, dy: 0);
            if (TryApplyRotation(board, temp))
            {
                return;
            }
            
            ShiftTempBlocks(temp, dx: 2, dy: 0);
            if (TryApplyRotation(board, temp))
            {
                return;
            }
            
            ShiftTempBlocks(temp, dx: -1, dy: -1);
            TryApplyRotation(board, temp);
        }
        
        private void ShiftTempBlocks(int[,] temp, int dx, int dy)
        {
            for (int i = 0; i < 4; i++)
            {
                temp[i, 0] += dx;
                temp[i, 1] += dy;
            }
        }

        private bool TryApplyRotation(Board board, int[,] temp)
        {
            if (!IsValidPosition(board, temp))
            {
                return false;
            }
            
            ApplyTempBlocks(temp);
            return true;
        }

        private bool IsValidPosition(Board board, int[,] temp)
        {
            for (int i = 0; i < 4; i++)
            {
                int x = temp[i, 0];
                int y = temp[i, 1];
                bool outOfBounds = x < 0 || x >= Board.Width || y < 0 || y >= Board.Height;
                bool hitBlock = board.IsOccupied(x, y);
                if (outOfBounds || hitBlock)
                {
                    return false;
                }
            }
            return true;
        }

        private void ApplyTempBlocks(int[,] temp)
        {
            for (int i = 0; i < 4; i++)
            {
                _blocks[i, 0] = temp[i, 0];
                _blocks[i, 1] = temp[i, 1];
            }
        }
    }
}
