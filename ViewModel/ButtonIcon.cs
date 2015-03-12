using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace a9t9Ocr
{
    public class ButtonIcon : Button
    {
        static ButtonIcon()
        {

        }

        public ImageSource ImageSource
        {
            get { return (ImageSource) GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ImageSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register(@"ImageSource", typeof (ImageSource), typeof (ButtonIcon),
                new UIPropertyMetadata(null));
    }
}
