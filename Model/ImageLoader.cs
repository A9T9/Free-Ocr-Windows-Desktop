using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;

namespace a9t9Ocr
{
    class ImageLoader
    {
        public List<ImageClass> LoadImages(List<string> paths)
        {
            var imagesList = new List<ImageClass>();
            foreach (var name in paths)
            {
                try
                {
                    var uri = new Uri(name);
                    var bitmap = new BitmapImage(uri);
                    imagesList.Add(
                        new ImageClass
                        {
                            FilePath = name,
                            Image = bitmap
                        });
                }
                catch
                {
                    // ignored
                }
            }
            return imagesList;
        }
    }
}
