using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using Tesseract;

namespace a9t9Ocr
{
    class TesseractOcr : ITesseractOrc
    {
        public string Language { get; set; }
        private readonly string _pathToTestData = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + @"\(a9t9)FreeOcr\tessdata";

        public TesseractOcr(string lang)
        {
            Language = lang;
        }

        public void ChangeLanguage(string lang)
        {
            Language = lang;
        }
        public string RecognizeOneImage(ImageClass image)
        {
            if (image == null)
                return string.Empty;
            return BeginRecognize(image.FilePath);
        }

        public List<string> RecognizeFewImages(List<ImageClass> images)
        {
            return images.Select(imageClass => BeginRecognize(imageClass.FilePath)).ToList();
        }
        
        private string BeginRecognize(string pathToImage)
        {
            try
            {
                if (!File.Exists(pathToImage))
                    return "Image not found";

                using (var engine = new TesseractEngine(_pathToTestData, Language, EngineMode.Default))
                {
                    using (var img = Pix.LoadFromFile(pathToImage))
                    {
                        using (var page = engine.Process(img))
                        {
                            var resultText = page.GetText();
                            if (!String.IsNullOrEmpty(resultText))
                                return resultText;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                MessageBox.Show(e.StackTrace);
                return null;
            }
            return null;
        }
    }
}
