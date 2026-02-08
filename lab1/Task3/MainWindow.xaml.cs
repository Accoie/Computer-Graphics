using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Task3
{
    public partial class MainWindow : Window
    {
        private readonly Color borderColor = Colors.Peru;
        private readonly Color fillColor = Colors.LightGoldenrodYellow;

        private int borderThickness = 2;
        private Point circleCenter;
        private double circleRadius = 150;

        private WriteableBitmap bitmap;
        private int bitmapWidth;
        private int bitmapHeight;
        private byte[] pixelData;

        public MainWindow()
        {
            InitializeComponent();

            thicknessSlider.Value = borderThickness;
            thicknessValueText.Text = borderThickness.ToString();

            thicknessSlider.ValueChanged += ( s, e ) =>
            {
                borderThickness = ( int )Math.Round( thicknessSlider.Value );
                thicknessValueText.Text = borderThickness.ToString();
                RedrawCircle();
            };

            ContentRendered += ( s, e ) =>
            {
                InitializeBitmap();
                RedrawCircle();
            };
        }

        private void InitializeBitmap()
        {
            bitmapWidth = ( int )canvas.ActualWidth;
            bitmapHeight = ( int )canvas.ActualHeight;

            if ( bitmapWidth <= 0 || bitmapHeight <= 0 )
            {
                return;
            }

            circleCenter = new Point( bitmapWidth / 2.0, bitmapHeight / 2.0 );

            bitmap = new WriteableBitmap(
                bitmapWidth,
                bitmapHeight,
                96, 96,
                PixelFormats.Bgra32,
                null );

            var image = new Image
            {
                Source = bitmap,
                Stretch = Stretch.None
            };

            canvas.Children.Clear();
            canvas.Children.Add( image );

            pixelData = new byte[ bitmapWidth * bitmapHeight * 4 ];
        }

        private void RedrawCircle()
        {
            if ( bitmap == null || pixelData == null )
            {
                return;
            }

            for ( int i = 0; i < pixelData.Length; i += 4 )
            {
                pixelData[ i ] = 255;
                pixelData[ i + 1 ] = 255;
                pixelData[ i + 2 ] = 255;
                pixelData[ i + 3 ] = 255;
            }

            int cx = ( int )Math.Round( circleCenter.X );
            int cy = ( int )Math.Round( circleCenter.Y );

            FillCircle( cx, cy, circleRadius - borderThickness );

            DrawCircleBorder( cx, cy, circleRadius );

            bitmap.WritePixels(
                new Int32Rect( 0, 0, bitmapWidth, bitmapHeight ),
                pixelData,
                bitmapWidth * 4,
                0 );
        }

        private void FillCircle( int cx, int cy, double radius )
        {
            if ( radius <= 0 )
            {
                return;
            }

            int minY = Math.Max( 0, cy - ( int )Math.Ceiling( radius ) );
            int maxY = Math.Min( bitmapHeight - 1, cy + ( int )Math.Ceiling( radius ) );

            for ( int y = minY; y <= maxY; y++ )
            {
                double dy = y - cy;
                double dx = Math.Sqrt( Math.Max( 0, radius * radius - dy * dy ) );

                int startX = ( int )Math.Ceiling( cx - dx );
                int endX = ( int )Math.Floor( cx + dx );

                startX = Math.Max( 0, startX );
                endX = Math.Min( bitmapWidth - 1, endX );

                for ( int x = startX; x <= endX; x++ )
                {
                    SetPixel( x, y, fillColor );
                }
            }
        }

        private void DrawCircleBorder( int cx, int cy, double radius )
        {
            if ( borderThickness <= 0 )
            {
                return;
            }

            for ( int t = 0; t < borderThickness; t++ )
            {
                double currentRadius = radius - t;
                DrawCircleOutline( cx, cy, currentRadius );
            }
        }

        private void DrawCircleOutline( int cx, int cy, double radius )
        {
            int x = 0;
            int y = ( int )Math.Round( radius );
            int d = 3 - 2 * ( int )Math.Round( radius );

            while ( y >= x )
            {
                SetBorderPixel( cx + x, cy + y );
                SetBorderPixel( cx - x, cy + y );
                SetBorderPixel( cx + x, cy - y );
                SetBorderPixel( cx - x, cy - y );
                SetBorderPixel( cx + y, cy + x );
                SetBorderPixel( cx - y, cy + x );
                SetBorderPixel( cx + y, cy - x );
                SetBorderPixel( cx - y, cy - x );

                x++;

                if ( d > 0 )
                {
                    y--;
                    d = d + 4 * ( x - y ) + 10;
                }
                else
                {
                    d = d + 4 * x + 6;
                }
            }
        }

        private void SetPixel( int x, int y, Color color )
        {
            if ( x < 0 || x >= bitmapWidth || y < 0 || y >= bitmapHeight )
            {
                return;
            }

            int index = ( y * bitmapWidth + x ) * 4;

            pixelData[ index ] = color.B;
            pixelData[ index + 1 ] = color.G;
            pixelData[ index + 2 ] = color.R;
            pixelData[ index + 3 ] = 255;
        }

        private void SetBorderPixel( int x, int y )
        {
            if ( x < 0 || x >= bitmapWidth || y < 0 || y >= bitmapHeight )
            {
                return;
            }

            int index = ( y * bitmapWidth + x ) * 4;

            pixelData[ index ] = borderColor.B;
            pixelData[ index + 1 ] = borderColor.G;
            pixelData[ index + 2 ] = borderColor.R;
            pixelData[ index + 3 ] = 255;
        }
    }
}