using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Task2
{
    public class ColorPickerDialog : Window
    {
        public Color SelectedColor { get; private set; }

        public ColorPickerDialog( Color initialColor )
        {
            Title = "Выбор цвета";
            Width = 300;
            Height = 380;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            ResizeMode = ResizeMode.NoResize;

            var colors = new[]
            {
                Colors.Black, Colors.White, Colors.Red, Colors.Green, Colors.Blue,
                Colors.Yellow, Colors.Cyan, Colors.Magenta, Colors.Orange, Colors.Purple,
                Colors.Brown, Colors.Gray, Colors.Pink, Colors.Lime, Colors.Turquoise
            };

            var wrap = new WrapPanel { Margin = new Thickness( 10 ) };
            foreach ( var color in colors )
            {
                var border = new Border
                {
                    Background = new SolidColorBrush( color ),
                    Width = 40,
                    Height = 40,
                    Margin = new Thickness( 3 ),
                    CornerRadius = new CornerRadius( 4 ),
                    Cursor = Cursors.Hand,
                    Tag = color
                };
                border.MouseLeftButtonDown += ( s, e ) =>
                {
                    SelectedColor = ( Color )border.Tag;
                    DialogResult = true;
                    Close();
                };
                wrap.Children.Add( border );
            }

            Content = new StackPanel
            {
                Margin = new Thickness( 10 ),
                Children =
                {
                    new TextBlock { Text = "Выберите цвет кисти:", Margin = new Thickness(0, 0, 0, 10) },
                    wrap
                }
            };

            SelectedColor = initialColor;
        }
    }
}