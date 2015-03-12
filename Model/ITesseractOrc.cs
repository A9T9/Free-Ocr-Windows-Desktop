using System.Collections.Generic;

namespace a9t9Ocr
{
    public interface ITesseractOrc
    {
        string RecognizeOneImage(ImageClass image);
        List<string> RecognizeFewImages(List<ImageClass> images);
        void ChangeLanguage(string lang);
    }
}
