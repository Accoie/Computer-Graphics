namespace Task2
{
    public enum ImageFormat
    {
        Jpeg,
        Bmp,
        Png
    }

    public static class ImageFormatTool
    {
        public static ImageFormat GetImageFormat( string extension )
        {
            extension = extension.ToLower();

            switch ( extension )
            {
                case ".jpg":
                case ".jpeg":
                    return ImageFormat.Jpeg;
                case ".bmp":
                    return ImageFormat.Bmp;
                default:
                    return ImageFormat.Png;
            }
        }

        public static System.Windows.Media.Imaging.BitmapEncoder CreateEncoder( ImageFormat format, int jpegQuality = 90 )
        {
            switch ( format )
            {
                case ImageFormat.Jpeg:
                    return new System.Windows.Media.Imaging.JpegBitmapEncoder
                    {
                        QualityLevel = jpegQuality
                    };
                case ImageFormat.Bmp:
                    return new System.Windows.Media.Imaging.BmpBitmapEncoder();
                default:
                    return new System.Windows.Media.Imaging.PngBitmapEncoder();
            }
        }
    }
}