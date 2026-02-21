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
    public partial class MainWindow : Window
    {
        private const int CanvasWidth = 1100;
        private const int CanvasHeight = 700;
        private const double MinOffset = 1.0;
        private const double DefaultBrushThickness = 5;

        private const int DpiX = 96;
        private const int DpiY = 96;

        private const int JpegQualityLevel = 90;

        private readonly Color CanvasBackgroundColor = Colors.White;

        private static readonly string[] imageOpenFormats = { "*.png", "*.jpg", "*.jpeg", "*.bmp" };
        private static readonly string[] imageSaveFormats = { "*.png", "*.jpg", "*.bmp" };

        private readonly string ImageFilesFilter;
        private readonly string SaveFilesFilter;

        private const string WindowTitle = "Image Painter";
        private const string NewCanvasTitle = "Image Painter - New Canvas";
        private const string SaveDialogTitle = "Сохранить рисунок";

        private bool isDrawing = false;
        private Point lastPoint;
        private Polyline currentPath;
        private Color drawingColor = Colors.Black;
        private double brushThickness = DefaultBrushThickness;

        public MainWindow()
        {
            InitializeComponent();

            // Используем универсальную функцию для создания фильтров
            ImageFilesFilter = CreateFileFilter( imageOpenFormats, includeAllFilesFilter: true );
            SaveFilesFilter = CreateFileFilter( imageSaveFormats, includeAllFilesFilter: false );

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
                Filter = SaveFilesFilter,
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
            var dialog = new ColorPickerDialog( drawingColor );

            if ( dialog.ShowDialog() == true )
            {
                drawingColor = dialog.SelectedColor;
            }
        }

        private void MenuItem_Exit_Click( object sender, RoutedEventArgs e )
        {
            Close();
        }

        private void DrawingCanvas_MouseLeftButtonDown( object sender, MouseButtonEventArgs e )
        {
            isDrawing = true;
            lastPoint = e.GetPosition( DrawingCanvas );

            currentPath = new Polyline
            {
                Stroke = new SolidColorBrush( drawingColor ),
                StrokeThickness = brushThickness,
                StrokeLineJoin = PenLineJoin.Round,
                StrokeStartLineCap = PenLineCap.Round,
                StrokeEndLineCap = PenLineCap.Round
            };

            currentPath.Points.Add( lastPoint );
            DrawingCanvas.Children.Add( currentPath );

            Mouse.Capture( DrawingCanvas );
            e.Handled = true;
        }

        private void DrawingCanvas_MouseMove( object sender, MouseEventArgs e )
        {
            if ( !isDrawing || e.LeftButton != MouseButtonState.Pressed )
            {
                return;
            }

            Point currentPoint = e.GetPosition( DrawingCanvas );

            bool outOfBounds = currentPoint.X < 0 || currentPoint.Y < 0 ||
                currentPoint.X > DrawingCanvas.ActualWidth ||
                currentPoint.Y > DrawingCanvas.ActualHeight;

            if ( outOfBounds )
            {
                return;
            }

            double dx = currentPoint.X - lastPoint.X;
            double dy = currentPoint.Y - lastPoint.Y;

            bool inBounds = dx * dx + dy * dy > MinOffset;

            if ( inBounds )
            {
                currentPath.Points.Add( currentPoint );
                lastPoint = currentPoint;
            }

            e.Handled = true;
        }

        private void Window_MouseLeftButtonUp( object sender, MouseButtonEventArgs e )
        {
            if ( isDrawing )
            {
                isDrawing = false;
                currentPath = null;
                Mouse.Capture( null );
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
                dc.DrawRectangle( new SolidColorBrush( CanvasBackgroundColor ), null, new Rect( 0, 0, width, height ) );
            }
            bitmap.Render( dv );

            ImageView.Source = bitmap;
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

        private string ShowOpenFileDialog()
        {
            var dialog = new OpenFileDialog { Filter = ImageFilesFilter };
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

        private void SaveBitmapToFile( RenderTargetBitmap bitmap, string filePath )
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
    }
}