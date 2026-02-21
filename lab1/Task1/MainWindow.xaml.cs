using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Task1
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded( object sender, RoutedEventArgs e )
        {
            await StartWaveAnimation();
        }

        private async Task StartWaveAnimation()
        {
            _ = AnimateCanvasContinuously( CanvasM );
            await Task.Delay( 700 );

            _ = AnimateCanvasContinuously( CanvasP );
            await Task.Delay( 400 );

            _ = AnimateCanvasContinuously( CanvasSh );
        }

        private async Task AnimateCanvasContinuously( Canvas canvas )
        {
            var translateTransform = new TranslateTransform();
            canvas.RenderTransform = translateTransform;
            // исправить делеи
            double moveDistance = 50; 
            double duration = 800;     
            int frameDelay = 16;       

            while ( IsLoaded )
            {
                double offset = moveDistance * frameDelay / duration;
                for ( double y = 0; y >= -moveDistance; y -= offset )
                {
                    if ( !IsLoaded )
                    {
                        translateTransform.Y = 0;
                        return;
                    }

                    translateTransform.Y = y;
                    await Task.Delay( frameDelay );
                }

                for ( double y = -moveDistance; y <= 0; y += offset)
                {
                    if ( !IsLoaded )
                    {
                        translateTransform.Y = 0;
                        return;
                    }

                    translateTransform.Y = y;
                    await Task.Delay( frameDelay );
                }
            }
        }
    }
}