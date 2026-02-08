using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

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
            await Task.Delay( 400 );

            _ = AnimateCanvasContinuously( CanvasP );
            await Task.Delay( 400 );

            _ = AnimateCanvasContinuously( CanvasSh );
        }

        private async Task AnimateCanvasContinuously( Canvas canvas )
        {
            double initialTop = canvas.Margin.Top;
            double moveDistance = 50;
            double duration = 2000;

            while ( true )
            {
                double steps = duration / 16;
                double stepDistance = moveDistance / ( steps / 2 );

                for ( int i = 0; i < steps / 2; i++ )
                {
                    if ( !IsLoaded )
                        return;

                    canvas.Margin = new Thickness(
                        canvas.Margin.Left,
                        canvas.Margin.Top - stepDistance,
                        canvas.Margin.Right,
                        canvas.Margin.Bottom );
                    await Task.Delay( 16 );
                }

                for ( int i = 0; i < steps / 2; i++ )
                {
                    if ( !IsLoaded )
                        return;

                    canvas.Margin = new Thickness(
                        canvas.Margin.Left,
                        canvas.Margin.Top + stepDistance,
                        canvas.Margin.Right,
                        canvas.Margin.Bottom );
                    await Task.Delay( 16 );
                }

                canvas.Margin = new Thickness( canvas.Margin.Left, initialTop, canvas.Margin.Right, canvas.Margin.Bottom );
            }
        }
    }
}