using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Novacode;

namespace a9t9Ocr
{
    class FileManipulator : IFileManipulator
    {
        public void SaveToFile(string fileName, string text)
        {
            try
            {
                if (text == null || fileName == null)
                    return;
                text = text.Replace("\n", Environment.NewLine);
                using (FileStream fs = new FileStream(fileName, FileMode.Append, FileAccess.Write))
                using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
                {
                    sw.WriteLine(text);
                }
            }
            catch
            {
                // ignored
            }
        }

        public void SaveToFile(string fileName, IEnumerable<string> text)
        {
            foreach (var tempText in text)
                SaveToFile(fileName, tempText);
        }

        public void SaveToWord(string fileName, string text)
        {
            try
            {
                if (text == null || fileName == null)
                    return;

                var doc = DocX.Create(fileName);
                doc.InsertParagraph(text);
                doc.Save();
            }
            catch
            {
                // ignored
            }
        }
    }
}
