using System;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;

namespace Task3
{
    public class MorphingGame : GameWindow
    {
        private Shader _shader;
        private Shape _shape;
        
        private float _progress = 0.0f;
        private float _direction = 1.0f;
        private const float SPEED = 0.002f;
        
        private float _rotationX = 0.0f;
        private float _rotationY = 0.0f;
        private float _scale = 1.0f;
        
        private Matrix4 _viewMatrix;
        private Matrix4 _projectionMatrix;
        private Matrix4 _modelMatrix;
        
        private Vector2 _lastMousePos;
        
        private Vector3 _lightDir = new Vector3(1, 1, 1).Normalized();

        public MorphingGame(GameWindowSettings gwSettings, NativeWindowSettings nwSettings)
            : base(gwSettings, nwSettings)
        {
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            
            GL.ClearColor(0.1f, 0.1f, 0.15f, 1.0f);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
            
            string shaderDir = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Shaders");
            _shader = new Shader(
                System.IO.Path.Combine(shaderDir, "vertex.glsl"),
                System.IO.Path.Combine(shaderDir, "fragment.glsl")
            );
            
            _shape = new Shape(_shader);
            UpdateViewMatrix();
            UpdateProjectionMatrix();
            
            CursorState = CursorState.Grabbed;
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, e.Width, e.Height);
            UpdateProjectionMatrix();
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
            
            if (KeyboardState.IsKeyDown(Keys.Escape)) Close();
            
            _progress += _direction * SPEED * (float)args.Time * 60;
            if (_progress >= 1.0f) { _progress = 1.0f; _direction = -1.0f; }
            else if (_progress <= 0.0f) { _progress = 0.0f; _direction = 1.0f; }
            
            if (MouseState.IsButtonDown(MouseButton.Left))
            {
                var mousePos = MousePosition;
                float dx = mousePos.X - _lastMousePos.X;
                float dy = mousePos.Y - _lastMousePos.Y;
                _rotationY += dx * 0.5f;
                _rotationX += dy * 0.5f;
            }
            _lastMousePos = MousePosition;
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            _shader.Use();
            
            UpdateModelMatrix();
            
            _shader.SetUniform("progress", _progress);
            _shader.SetUniform("model", _modelMatrix);
            _shader.SetUniform("view", _viewMatrix);
            _shader.SetUniform("projection", _projectionMatrix);
            _shader.SetUniform("lightDir", _lightDir);
            
            _shape.Draw();
            SwapBuffers();
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            _scale = Clamp(_scale + e.OffsetY * 0.1f, 0.1f, 10.0f);
        }

        private void UpdateModelMatrix()
        {
            _modelMatrix = Matrix4.Identity;
            _modelMatrix = Matrix4.CreateScale(_scale) * _modelMatrix;
            _modelMatrix = Matrix4.CreateRotationY(MathHelper.DegreesToRadians(_rotationY)) * _modelMatrix;
            _modelMatrix = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(_rotationX)) * _modelMatrix;
        }

        private void UpdateViewMatrix()
        {
            _viewMatrix = Matrix4.LookAt(
                new Vector3(0, 0, 3), Vector3.Zero, Vector3.UnitY);
        }

        private void UpdateProjectionMatrix()
        {
            float aspect = Size.X / (float)Size.Y;
            _projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(
                MathHelper.DegreesToRadians(45), aspect, 0.1f, 100.0f);
        }

        protected override void OnUnload()
        {
            _shape?.Dispose();
            _shader?.Dispose();
            base.OnUnload();
        }

        private static float Clamp(float v, float min, float max) => v < min ? min : v > max ? max : v;
    }
}
