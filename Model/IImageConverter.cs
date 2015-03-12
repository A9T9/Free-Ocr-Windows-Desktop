using System.Collections.Generic;

namespace a9t9Ocr
{
    interface IImageConverter
    {
        List<string> PathToConvertedImages(string pathToPdf);
    }
}
