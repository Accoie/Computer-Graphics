using Task3.Enums;

namespace Task3.Models
{
    public enum ShapeType
    {
        Line,
        Square,
        BlockL,
        ZigZag,
        BlockT,
    }

    public readonly struct Block
    {
        public int X { get; init; }
        public int Y { get; init; }
        
        public Block(int x, int y)
        {
            X = x;
            Y = y;
        }
        
        public Block Shift(int dx, int dy) => new Block(X + dx, Y + dy);
    }

    public class Shape
    {
        private const int BlockCount = 4;

        private readonly Block[] _blocks = new Block[BlockCount];
        private ShapeType _shapeType;
        private ColorType _color;

        private static readonly ShapeType[] AllShapeTypes = Enum.GetValues<ShapeType>();
        private static readonly Random Rand = new();

        public Block this[int blockIndex] => _blocks[blockIndex];
        public ColorType Color => _color;
        public ShapeType ShapeType => _shapeType;

        public void SpawnNew()
        {
            _shapeType = AllShapeTypes[Rand.Next(AllShapeTypes.Length)];
            _color = (ColorType)(Rand.Next(6) + 2);
            SetInitialBlocks();
        }

        public static Shape GenerateNextShape()
        {
            var shape = new Shape();
            shape._shapeType = AllShapeTypes[Rand.Next(AllShapeTypes.Length)];
            shape._color = (ColorType)(Rand.Next(6) + 2);
            shape.SetInitialBlocks();
            return shape;
        }

        public void ApplyNext(Shape nextShape)
        {
            _shapeType = nextShape._shapeType;
            _color = nextShape._color;
            SetInitialBlocks();
        }

        public static Block[] GetPreviewBlocks(ShapeType type)
        {
            return type switch
            {
                ShapeType.Line => new Block[]
                {
                    new Block(0, 0), new Block(1, 0), new Block(2, 0), new Block(3, 0)
                },
                ShapeType.Square => new Block[]
                {
                    new Block(0, 0), new Block(1, 0), new Block(0, 1), new Block(1, 1)
                },
                ShapeType.BlockL => new Block[]
                {
                    new Block(0, 0), new Block(1, 0), new Block(2, 0), new Block(2, 1)
                },
                ShapeType.ZigZag => new Block[]
                {
                    new Block(1, 0), new Block(2, 0), new Block(0, 1), new Block(1, 1)
                },
                ShapeType.BlockT => new Block[]
                {
                    new Block(0, 0), new Block(1, 0), new Block(2, 0), new Block(1, 1)
                },
                _ => throw new ArgumentOutOfRangeException(nameof(type))
            };
        }

        public bool CheckCollisionWithField(Field field)
        {
            for (int i = 0; i < BlockCount; i++)
            {
                Block block = _blocks[i];
                bool blockCollides = block.Y >= 0 && field.IsOccupied(block.X, block.Y);
                if (blockCollides)
                {
                    return true;
                }
            }

            return false;
        }

        public bool TryMoveDown(Field field)
        {
            if (!CanMoveDown(field))
            {
                PlaceOnField(field);
                return false;
            }

            ShiftBlocks(dx: 0, dy: 1);
            return true;
        }

        public void MoveRight(Field field)
        {
            if (CanMoveHorizontally(field, 1))
            {
                ShiftBlocks(dx: 1, dy: 0);
            }
        }

        public void MoveLeft(Field field)
        {
            if (CanMoveHorizontally(field, -1))
            {
                ShiftBlocks(dx: -1, dy: 0);
            }
        }

        public void Rotate(Field field)
        {
            if (_shapeType == ShapeType.Square)
            {
                return;
            }

            Block[] temp = CopyBlocks();

            if (_shapeType == ShapeType.Line)
            {
                RotateIShape(temp);
            }
            else
            {
                RotateAroundCenter(temp);
            }

            TryWallKicks(field, temp);
        }

        public bool FastDrop(Field field)
        {
            if (!CanMoveDown(field))
            {
                PlaceOnField(field);
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
            _blocks[0] = new Block(3, 0);
            _blocks[1] = new Block(4, 0);
            _blocks[2] = new Block(5, 0);
            _blocks[3] = new Block(6, 0);
        }

        private void SetSquareShape()
        {
            _blocks[0] = new Block(4, 0);
            _blocks[1] = new Block(5, 0);
            _blocks[2] = new Block(4, 1);
            _blocks[3] = new Block(5, 1);
        }

        private void SetBlockLShape()
        {
            _blocks[0] = new Block(4, 0);
            _blocks[1] = new Block(5, 0);
            _blocks[2] = new Block(6, 0);
            _blocks[3] = new Block(6, 1);
        }

        private void SetZigZagShape()
        {
            _blocks[0] = new Block(5, 0);
            _blocks[1] = new Block(6, 0);
            _blocks[2] = new Block(4, 1);
            _blocks[3] = new Block(5, 1);
        }

        private void SetBlockTShape()
        {
            _blocks[0] = new Block(4, 0);
            _blocks[1] = new Block(5, 0);
            _blocks[2] = new Block(6, 0);
            _blocks[3] = new Block(5, 1);
        }

        private bool CanMoveDown(Field field)
        {
            for (int i = 0; i < BlockCount; i++)
            {
                Block block = _blocks[i];
                int newY = block.Y + 1;
                bool hitBottom = newY >= Field.Height;
                bool hitBlock = field.IsOccupied(block.X, newY);

                if (hitBottom || hitBlock)
                {
                    return false;
                }
            }

            return true;
        }

        private bool CanMoveHorizontally(Field field, int dx)
        {
            for (int i = 0; i < BlockCount; i++)
            {
                Block block = _blocks[i];
                int newX = block.X + dx;
                bool outOfBounds = newX is < 0 or >= Field.Width;
                bool hitBlock = field.IsOccupied(newX, block.Y);

                if (outOfBounds || hitBlock)
                {
                    return false;
                }
            }

            return true;
        }

        private void ShiftBlocks(int dx, int dy)
        {
            for (int i = 0; i < BlockCount; i++)
            {
                _blocks[i] = _blocks[i].Shift(dx, dy);
            }
        }

        private void PlaceOnField(Field field)
        {
            for (int i = 0; i < BlockCount; i++)
            {
                Block block = _blocks[i];
                bool insideField = block.X is >= 0 and < Field.Width && block.Y is >= 0 and < Field.Height;

                if (insideField)
                {
                    field[block.X, block.Y] = _color;
                }
            }
        }

        private Block[] CopyBlocks()
        {
            var copy = new Block[BlockCount];
            for (int i = 0; i < BlockCount; i++)
            {
                copy[i] = _blocks[i];
            }

            return copy;
        }

        private void RotateAroundCenter(Block[] temp)
        {
            int cx = _blocks[1].X;
            int cy = _blocks[1].Y;

            for (int i = 0; i < BlockCount; i++)
            {
                int x = _blocks[i].X - cx;
                int y = _blocks[i].Y - cy;
                temp[i] = new Block(cx - y, cy + x);
            }
        }

        private void RotateIShape(Block[] temp)
        {
            int cx = _blocks[1].X;
            int cy = _blocks[1].Y;
            bool isHorizontal = _blocks[0].Y == _blocks[1].Y;

            if (isHorizontal)
            {
                temp[0] = new Block(cx, cy + 1);
                temp[1] = new Block(cx, cy);
                temp[2] = new Block(cx, cy - 1);
                temp[3] = new Block(cx, cy - 2);
            }
            else
            {
                temp[0] = new Block(cx - 1, cy);
                temp[1] = new Block(cx, cy);
                temp[2] = new Block(cx + 1, cy);
                temp[3] = new Block(cx + 2, cy);
            }
        }

        private void TryWallKicks(Field field, Block[] temp)
        {
            if (TryApplyRotation(field, temp))
            {
                return;
            }

            ShiftTempBlocks(temp, dx: -1, dy: 0);
            if (TryApplyRotation(field, temp))
            {
                return;
            }

            ShiftTempBlocks(temp, dx: 2, dy: 0);
            if (TryApplyRotation(field, temp))
            {
                return;
            }

            ShiftTempBlocks(temp, dx: -1, dy: -1);
            TryApplyRotation(field, temp);
        }

        private void ShiftTempBlocks(Block[] temp, int dx, int dy)
        {
            for (int i = 0; i < BlockCount; i++)
            {
                temp[i] = temp[i].Shift(dx, dy);
            }
        }

        private bool TryApplyRotation(Field field, Block[] temp)
        {
            if (!IsValidPosition(field, temp))
            {
                return false;
            }

            ApplyTempBlocks(temp);

            return true;
        }

        private bool IsValidPosition(Field field, Block[] temp)
        {
            for (int i = 0; i < BlockCount; i++)
            {
                Block block = temp[i];
                bool outOfBounds = block.X is < 0 or >= Field.Width || block.Y < 0 || block.Y >= Field.Height;
                bool hitBlock = field.IsOccupied(block.X, block.Y);
                if (outOfBounds || hitBlock)
                {
                    return false;
                }
            }

            return true;
        }

        private void ApplyTempBlocks(Block[] temp)
        {
            for (int i = 0; i < BlockCount; i++)
            {
                _blocks[i] = temp[i];
            }
        }
    }
}