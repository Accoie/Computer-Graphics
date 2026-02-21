using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Task1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly string[] imageFormats = { "*.png", "*.jpg", "*.bmp", "*.jpeg", "*.gif", "*.tiff" };
        private readonly string fileFormat;

        public MainWindow()
        {
            InitializeComponent();

            fileFormat = CreateFileFilter( imageFormats );
        }

        private string CreateFileFilter( string[] formats )
        {
            string filterPattern = string.Join( ";", formats );
            return $"Image Files ({filterPattern})|{filterPattern}|All files (*.*)|*.*";
        }

        private void MenuItem_Open_Click( object sender, RoutedEventArgs e )
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = fileFormat;

            if ( openFileDialog.ShowDialog() == true )
            {
                string selectedImagePath = openFileDialog.FileName;
                DisplaySelectedImage( selectedImagePath );
            }
        }

        private void DisplaySelectedImage( string imagePath )
        {
            try
            {
                BitmapImage bitmap = new BitmapImage( new Uri( imagePath ) );
                ImageView.Source = bitmap;
                ClearBackground();

                if ( System.IO.Path.GetExtension( imagePath ).ToLower() == ".png" )
                {
                    AddSemiTransparentBackground();
                }
                else
                {
                    ClearBackground();
                }
            }
            catch ( Exception )
            {
            }
        }

        private void AddSemiTransparentBackground()
        {
            ImageBrush imageBrush = new ImageBrush();
            imageBrush.ImageSource = new BitmapImage( new Uri( "background.png", UriKind.Relative ) );

            grid.Background = imageBrush;
        }
        private void ClearBackground()
        {
            Grid parentGrid = ( Grid )ImageView.Parent;

            foreach ( UIElement child in grid.Children )
            {
                if ( child is Canvas )
                {
                    grid.Children.Remove( child );
                    break;
                }
            }

            parentGrid.Background = Brushes.Transparent;
        }
    }
}