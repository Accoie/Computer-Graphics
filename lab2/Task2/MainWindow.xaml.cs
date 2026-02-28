using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Task2
{
    public partial class MainWindow
    {
        private const int CanvasWidth = 1100;
        private const int CanvasHeight = 700;
        private const double MinOffset = 1.0;
        private const double DefaultBrushThickness = 5;

        private const int DpiX = 96;
        private const int DpiY = 96;

        private const int JpegQualityLevel = 90;

        private readonly Color _canvasBackgroundColor = Colors.White;

        private static readonly string[] ImageOpenFormats = { "*.png", "*.jpg", "*.jpeg", "*.bmp" };
        private static readonly string[] ImageSaveFormats = { "*.png", "*.jpg", "*.bmp" };

        private readonly string _imageFilesFilter;
        private readonly string _saveFilesFilter;

        private const string WindowTitle = "Image Painter";
        private const string NewCanvasTitle = "Image Painter - New Canvas";
        private const string SaveDialogTitle = "Сохранить рисунок";

        private bool _isDrawing;
        private Point _lastPoint;
        private Polyline _currentPath;
        private Color _drawingColor = Colors.Black;
        private double _brushThickness = DefaultBrushThickness;

        public MainWindow()
        {
            InitializeComponent();

            _imageFilesFilter = CreateFileFilter( ImageOpenFormats, includeAllFilesFilter: true );
            _saveFilesFilter = CreateFileFilter( ImageSaveFormats, includeAllFilesFilter: false );

            CreateNewCanvas( CanvasWidth, CanvasHeight );
        }

        private void MenuItem_New_Click( object sender, RoutedEventArgs e )
        {
            CreateNewCanvas( CanvasWidth, CanvasHeight );
        }

        private void MenuItem_Open_Click( object sender, RoutedEventArgs e )
        {
            string filePath = ShowOpenFileDialog();

            if ( string.IsNullOrEmpty( filePath ) )
            {
                return;
            }

            try
            {
                BitmapImage image = LoadImageFromFile( filePath );
                ImageView.Source = image;
                ImageView.Width = image.PixelWidth;
                ImageView.Height = image.PixelHeight;
                DrawingCanvas.Width = image.PixelWidth;
                DrawingCanvas.Height = image.PixelHeight;
                DrawingGrid.Width = image.PixelWidth;
                DrawingGrid.Height = image.PixelHeight;
                DrawingCanvas.Children.Clear();
                string fileName = System.IO.Path.GetFileName( filePath );
                Title = $"{WindowTitle} - {fileName}";
            }
            catch ( Exception ex )
            {
                ShowErrorMessage( $"Ошибка загрузки: {ex.Message}" );
            }
        }

        private void MenuItem_SaveAs_Click( object sender, RoutedEventArgs e )
        {
            var dialog = new SaveFileDialog
            {
                Filter = _saveFilesFilter,
                Title = SaveDialogTitle
            };

            if ( dialog.ShowDialog() == true )
            {
                try
                {
                    PrepareDrawingGridForRender();
                    var renderBitmap = RenderDrawingGrid();
                    SaveBitmapToFile( renderBitmap, dialog.FileName );

                    ShowSuccessMessage();
                }
                catch ( Exception ex )
                {
                    ShowErrorMessage( $"Ошибка сохранения: {ex.Message}" );
                }
            }
        }

        private void MenuItem_ClearDrawing_Click( object sender, RoutedEventArgs e )
        {
            DrawingCanvas.Children.Clear();
        }

        private void MenuItem_SelectColor_Click( object sender, RoutedEventArgs e )
        {
            var dialog = new ColorPickerDialog( _drawingColor );

            if ( dialog.ShowDialog() == true )
            {
                _drawingColor = dialog.SelectedColor;
            }
        }

        private void MenuItem_Exit_Click( object sender, RoutedEventArgs e )
        {
            Close();
        }

        private void DrawingCanvas_MouseLeftButtonDown( object sender, MouseButtonEventArgs e )
        {
            _isDrawing = true;
            _lastPoint = e.GetPosition( DrawingCanvas );

            _currentPath = new Polyline
            {
                Stroke = new SolidColorBrush( _drawingColor ),
                StrokeThickness = _brushThickness,
                StrokeLineJoin = PenLineJoin.Round,
                StrokeStartLineCap = PenLineCap.Round,
                StrokeEndLineCap = PenLineCap.Round
            };

            _currentPath.Points.Add( _lastPoint );
            DrawingCanvas.Children.Add( _currentPath );

            Mouse.Capture( DrawingCanvas );
            e.Handled = true;
        }

        private void DrawingCanvas_MouseMove( object sender, MouseEventArgs e )
        {
            if ( !_isDrawing || e.LeftButton != MouseButtonState.Pressed )
            {
                return;
            }

            Point currentPoint = e.GetPosition( DrawingCanvas );
            ProcessDrawingMove( currentPoint );
            e.Handled = true;
        }

        private void Window_MouseMove( object sender, MouseEventArgs e )
        {
            if ( !_isDrawing || e.LeftButton != MouseButtonState.Pressed )
            {
                return;
            }

            Point currentPoint = e.GetPosition( DrawingCanvas );
            ProcessDrawingMove( currentPoint );
        }

        private void Window_MouseLeftButtonUp( object sender, MouseButtonEventArgs e )
        {
            if ( _isDrawing )
            {
                _isDrawing = false;
                _currentPath = null;
                Mouse.Capture( null );
            }
        }
        
        private void ProcessDrawingMove( Point currentPoint )
        {
            double clampedX = Math.Max( 0, Math.Min( currentPoint.X, DrawingCanvas.ActualWidth ) );
            double clampedY = Math.Max( 0, Math.Min( currentPoint.Y, DrawingCanvas.ActualHeight ) );
            var clampedPoint = new Point( clampedX, clampedY );

            double dx = clampedPoint.X - _lastPoint.X;
            double dy = clampedPoint.Y - _lastPoint.Y;

            bool shouldAddPoint = dx * dx + dy * dy > MinOffset;

            if ( shouldAddPoint )
            {
                _currentPath.Points.Add( clampedPoint );
                _lastPoint = clampedPoint;
            }
        }

        private string CreateFileFilter( string[] formats, bool includeAllFilesFilter = true )
        {
            var filterParts = new System.Collections.Generic.List<string>();

            
            foreach ( string format in formats )
            {
                string extension = format.Replace( "*", "" ).TrimStart( '.' );
                string description = extension.ToUpper() + $" (*.{extension})";
                filterParts.Add( $"{description}|{format}" );
            }

            if ( includeAllFilesFilter )
            {
                filterParts.Add( "All files (*.*)|*.*" );
            }

            return string.Join( "|", filterParts );
        }

        private void CreateNewCanvas( int width, int height )
        {
            var bitmap = new RenderTargetBitmap( width, height, DpiX, DpiY, PixelFormats.Pbgra32 );
            var dv = new DrawingVisual();
            using ( var dc = dv.RenderOpen() )
            {
                dc.DrawRectangle( new SolidColorBrush( _canvasBackgroundColor ), null, new Rect( 0, 0, width, height ) );
            }
            bitmap.Render( dv );

            ImageView.Source = bitmap;
            ImageView.Width = width;
            ImageView.Height = height;
            DrawingCanvas.Width = width;
            DrawingCanvas.Height = height;
            DrawingGrid.Width = width;
            DrawingGrid.Height = height;
            DrawingCanvas.Children.Clear();
            Title = NewCanvasTitle;
        }

        private BitmapImage LoadImageFromFile( string filePath )
        {
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri( filePath );
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();

            return bitmap;
        }

        private void PrepareDrawingGridForRender()
        {
            DrawingGrid.Measure( new Size( double.PositiveInfinity, double.PositiveInfinity ) );
            DrawingGrid.Arrange( new Rect( DrawingGrid.DesiredSize ) );
            DrawingGrid.UpdateLayout();
        }

        private RenderTargetBitmap RenderDrawingGrid()
        {
            var renderBitmap = new RenderTargetBitmap(
                ( int )DrawingGrid.ActualWidth,
                ( int )DrawingGrid.ActualHeight,
                DpiX, DpiY, PixelFormats.Pbgra32 );
            renderBitmap.Render( DrawingGrid );

            return renderBitmap;
        }

        private void SaveBitmapToFile( BitmapSource bitmap, string filePath )
        {
            using ( var stream = new FileStream( filePath, FileMode.Create ) )
            {
                BitmapEncoder encoder = GetEncoder( filePath );
                encoder.Frames.Add( BitmapFrame.Create( bitmap ) );
                encoder.Save( stream );
            }
        }

        private BitmapEncoder GetEncoder( string fileName )
        {
            string extension = System.IO.Path.GetExtension( fileName ).ToLower();

            ImageFormat format;
            format = ImageFormatTool.GetImageFormat( extension );

            switch ( format )
            {
                case ImageFormat.Jpeg:
                    return new JpegBitmapEncoder { QualityLevel = JpegQualityLevel };
                case ImageFormat.Bmp:
                    return new BmpBitmapEncoder();
                default:
                    return new PngBitmapEncoder();
            }
        }

        private string ShowOpenFileDialog()
        {
            var dialog = new OpenFileDialog { Filter = _imageFilesFilter };
            return dialog.ShowDialog() == true ? dialog.FileName : null;
        }

        private void ShowErrorMessage( string message )
        {
            MessageBox.Show( message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error );
        }

        private void ShowSuccessMessage()
        {
            MessageBox.Show( "Изображение сохранено", "Успех!",
                MessageBoxButton.OK, MessageBoxImage.Information );
        }
    }
}