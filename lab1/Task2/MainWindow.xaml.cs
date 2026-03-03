using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Task2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private bool _isDragging;
        private Point _lastMousePosition;
        private readonly TranslateTransform _transform;

        public MainWindow()
        {
            InitializeComponent();

            _transform = new TranslateTransform();
            SceneContainer.RenderTransform = _transform;
        }

        private void SceneContainer_MouseLeftButtonDown( object sender, MouseButtonEventArgs e )
        {
            _isDragging = true;
            _lastMousePosition = e.GetPosition( this );
            SceneContainer.CaptureMouse();
            Cursor = Cursors.SizeAll;
        }

        private void SceneContainer_MouseMove( object sender, MouseEventArgs e )
        {
            if ( _isDragging && SceneContainer.IsMouseCaptured )
            {
                Point currentMousePosition = e.GetPosition( this );
                double deltaX = currentMousePosition.X - _lastMousePosition.X;
                double deltaY = currentMousePosition.Y - _lastMousePosition.Y;

                _transform.X += deltaX;
                _transform.Y += deltaY;

                _lastMousePosition = currentMousePosition;
            }
        }

        private void SceneContainer_MouseLeftButtonUp( object sender, MouseButtonEventArgs e )
        {
            _isDragging = false;
            SceneContainer.ReleaseMouseCapture();
            Cursor = Cursors.Arrow;
        }
    }
}