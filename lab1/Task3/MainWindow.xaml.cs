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

            int xc = ( int )Math.Round( circleCenter.X );
            int yc = ( int )Math.Round( circleCenter.Y );

            DrawCircle( xc, yc, ( int )circleRadius, borderThickness );

            bitmap.WritePixels(
                new Int32Rect( 0, 0, bitmapWidth, bitmapHeight ),
                pixelData,
                bitmapWidth * 4,
                0 );
        }

        private void DrawCircle( int xc, int yc, int r, int thickness )
        {
            int x0 = xc - r - 2;
            int x1 = xc + r + 2;
            int y0 = yc - r - 2;
            int y1 = yc + r + 2;

            x0 = Math.Max( 0, x0 );
            x1 = Math.Min( bitmapWidth - 1, x1 );
            y0 = Math.Max( 0, y0 );
            y1 = Math.Min( bitmapHeight - 1, y1 );

            double outerRadius = r;
            double innerRadius = r - thickness;
            // выяснить как то упростить(или декомпозировать)
            for ( int y = y0; y <= y1; y++ )
            {
                for ( int x = x0; x <= x1; x++ )
                {
                    double dx = x - xc;
                    double dy = y - yc;
                    double distance = Math.Sqrt( dx * dx + dy * dy );

                    if ( thickness > 0 )
                    {
                        double alpha = 0.0;

                        if ( thickness <= 2 )
                        {
                            if ( distance > outerRadius && distance < outerRadius + 1.0 )
                            {
                                alpha = 1.0 - ( distance - outerRadius );
                            }
                            else if ( distance < innerRadius && distance > innerRadius - 1.0 )
                            {
                                alpha = 1.0 - ( innerRadius - distance );
                            }
                            else if ( distance >= innerRadius && distance <= outerRadius )
                            {
                                alpha = 1.0;
                            }
                        }
                        else
                        {
                            double centerRadius = ( outerRadius + innerRadius ) / 2.0;
                            double halfThickness = thickness / 2.0;
                            double distFromCenter = Math.Abs( distance - centerRadius );

                            if ( distFromCenter <= halfThickness )
                            {
                                alpha = 1.0;
                            }
                            else if ( distFromCenter <= halfThickness + 1.0 )
                            {
                                alpha = 1.0 - ( distFromCenter - halfThickness );
                            }
                        }

                        if ( alpha > 0.0 )
                        {
                            BlendPixel( x, y, borderColor, alpha );
                            continue;
                        }
                    }

                    if ( distance <= innerRadius )
                    {
                        SetPixelSolid( x, y, fillColor );
                    }
                }
            }
        }

        private void SetPixelSolid( int x, int y, Color color )
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

        private void BlendPixel( int x, int y, Color color, double alpha )
        {
            if ( x < 0 || x >= bitmapWidth || y < 0 || y >= bitmapHeight )
            {
                return;
            }

            int index = ( y * bitmapWidth + x ) * 4;

            byte dstB = pixelData[ index ];
            byte dstG = pixelData[ index + 1 ];
            byte dstR = pixelData[ index + 2 ];

            byte srcB = color.B;
            byte srcG = color.G;
            byte srcR = color.R;

            pixelData[ index ] = ( byte )( srcB * alpha + dstB * ( 1.0 - alpha ) );
            pixelData[ index + 1 ] = ( byte )( srcG * alpha + dstG * ( 1.0 - alpha ) );
            pixelData[ index + 2 ] = ( byte )( srcR * alpha + dstR * ( 1.0 - alpha ) );
            pixelData[ index + 3 ] = 255;
        }
    }
}