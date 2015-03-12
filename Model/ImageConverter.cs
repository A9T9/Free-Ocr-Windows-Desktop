using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Ghostscript.NET;
using Ghostscript.NET.Rasterizer;

namespace a9t9Ocr
{
    class ImageConverter : IImageConverter
    {
        private string _oldDirectory = string.Empty;
        private int _count = 0;
        public void ClearTempImages()
        {
            try
            {
                if (Directory.Exists(_oldDirectory + _count))
                    Directory.Delete(_oldDirectory + _count, true);
               
            }
            catch
            {
                // ignored
            }
            finally
            {
                Directory.CreateDirectory(_oldDirectory + ++_count);
            }
        }
        public List<string> PathToConvertedImages(string pathToPdf)
        {
            var resultImages = new List<string>();
            try
            {
                const int desiredXDpi = 96;
                const int desiredYDpi = 96;

                string inputPdfPath = pathToPdf;
                var directoryInfo = new FileInfo(inputPdfPath).Directory;
                if (directoryInfo == null) return resultImages;

                string outputPath = directoryInfo.FullName + "\\tempimages\\";
                _oldDirectory = outputPath;
                ClearTempImages();

                var lastInstalledVersion = GhostscriptVersionInfo.GetLastInstalledVersion(
                    GhostscriptLicense.GPL | GhostscriptLicense.AFPL,
                    GhostscriptLicense.GPL);

                var rasterizer = new GhostscriptRasterizer();
                rasterizer.Open(inputPdfPath, lastInstalledVersion, false);

                for (int pageNumber = 1; pageNumber <= rasterizer.PageCount; pageNumber++)
                {
                    string pageFilePath = Path.Combine(outputPath + _count + @"\", @"Page-" + pageNumber + @".tiff");
                    Image img = rasterizer.GetPage(desiredXDpi, desiredYDpi, pageNumber);
                    img.Save(pageFilePath, ImageFormat.Tiff);
                    resultImages.Add(pageFilePath);
                }

            }
            catch 
            {
                //ignored    
            }

            return resultImages;
        }
    }
}
