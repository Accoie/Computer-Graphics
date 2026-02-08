using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Task2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _isDragging = false;
        private Point _lastMousePosition;
        private TranslateTransform _transform;

        public MainWindow()
        {
            InitializeComponent();

            // Инициализируем трансформацию для плавного перемещения
            _transform = new TranslateTransform();
            sceneContainer.RenderTransform = _transform;
        }

        private void SceneContainer_MouseLeftButtonDown( object sender, MouseButtonEventArgs e )
        {
            _isDragging = true;
            _lastMousePosition = e.GetPosition( this );
            sceneContainer.CaptureMouse();
            Cursor = Cursors.SizeAll; // Меняем курсор на "перемещение"
        }

        private void SceneContainer_MouseMove( object sender, MouseEventArgs e )
        {
            if ( _isDragging && sceneContainer.IsMouseCaptured )
            {
                Point currentMousePosition = e.GetPosition( this );
                double deltaX = currentMousePosition.X - _lastMousePosition.X;
                double deltaY = currentMousePosition.Y - _lastMousePosition.Y;

                // Применяем перемещение
                _transform.X += deltaX;
                _transform.Y += deltaY;

                _lastMousePosition = currentMousePosition;
            }
        }

        private void SceneContainer_MouseLeftButtonUp( object sender, MouseButtonEventArgs e )
        {
            _isDragging = false;
            sceneContainer.ReleaseMouseCapture();
            Cursor = Cursors.Arrow; // Возвращаем стандартный курсор
        }
    }
}