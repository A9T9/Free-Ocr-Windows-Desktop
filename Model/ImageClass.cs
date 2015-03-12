using System.Windows.Media.Imaging;

namespace a9t9Ocr
{
    public class ImageClass
    {
        public string FilePath { get; set; }

        public BitmapImage Image { get; set; }

        public ImageClass()
        {
            Image = new BitmapImage();
        }
    }
}
