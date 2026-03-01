using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Task3
{
    public class InputHandler
    {
        private const int InitialDelay = 15; 
        private const int RepeatDelay = 4;
        
        private readonly KeyState _leftKey = new();
        private readonly KeyState _rightKey = new();
        private readonly KeyState _spaceKey = new();
        private readonly KeyState _rKey = new();
        private readonly KeyState _upKey = new();

        public void Update(KeyboardState keyboard)
        {
            _upKey.Update(keyboard.IsKeyDown(Keys.W));
            _leftKey.Update(keyboard.IsKeyDown(Keys.A));
            _rightKey.Update(keyboard.IsKeyDown(Keys.D));
            _spaceKey.Update(keyboard.IsKeyDown(Keys.Space));
            _rKey.Update(keyboard.IsKeyDown(Keys.R));
        }

        public bool IsMoveLeftPressed() => _leftKey.IsPressed(InitialDelay, RepeatDelay);
        public bool IsMoveRightPressed() => _rightKey.IsPressed(InitialDelay, RepeatDelay);
        public bool IsRotatePressed() => _upKey.IsPressedOnce();
        public bool IsFastDropHeld() => _spaceKey.IsHeld();
        public bool IsRestartPressed() => _rKey.IsPressedOnce();

        private class KeyState
        {
            private int _timer;
            private bool _wasDown;

            public void Update(bool isDown)
            {
                if (isDown)
                {
                    _timer = !_wasDown ? 0 : _timer + 1;
                    _wasDown = true;
                }
                else
                {
                    _wasDown = false;
                    _timer = 0;
                }
            }

            public bool IsPressed(int initialDelay, int repeatDelay)
            {
                if (!_wasDown)
                {
                    return false;
                }
                
                bool isFirstPress = _timer == 0;
                bool passedInitialDelay = _timer >= initialDelay;
                bool isRepeatInterval = (_timer - initialDelay) % repeatDelay == 0;
                
                return isFirstPress || (passedInitialDelay && isRepeatInterval);
            }

            public bool IsPressedOnce()
            {
                return _wasDown && _timer == 0;
            }

            public bool IsHeld()
            {
                return _wasDown;
            }
        }
    }
}
