using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace a9t9Ocr
{
    public static class ExtensionsClass
    {
        public static Byte[] ImageToByte(this BitmapImage imageSource)
        {
            Stream stream = imageSource.StreamSource;
            Byte[] buffer = null;
            if (stream != null && stream.Length > 0)
            {
                using (BinaryReader br = new BinaryReader(stream))
                {
                    buffer = br.ReadBytes((Int32)stream.Length);
                }
            }

            return buffer;
        }
    }
}
