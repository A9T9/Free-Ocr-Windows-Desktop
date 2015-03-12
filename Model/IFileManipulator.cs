using System.Collections.Generic;

namespace a9t9Ocr
{
    interface IFileManipulator
    {
        void SaveToFile(string fileName, string text);
        void SaveToFile(string fileName, IEnumerable<string> text);
        void SaveToWord(string fileName, string text);
    }
}
