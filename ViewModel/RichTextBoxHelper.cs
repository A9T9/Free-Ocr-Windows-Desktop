using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace a9t9Ocr
{
    /// <summary>
    /// RichTextBox in wpf/mvvm sucks..
    /// i;m changed to simple textbox
    /// </summary>
    public class RichTextBoxHelper : DependencyObject
    {
        public static string GetDocumentXaml(DependencyObject obj)
        {
            return (string) obj.GetValue(DocumentXamlProperty);
        }

        public static void SetDocumentXaml(DependencyObject obj, string value)
        {
            obj.SetValue(DocumentXamlProperty, value);
        }

        public static void UpdateSource()
        {

        }

        public static readonly DependencyProperty DocumentXamlProperty =
            DependencyProperty.RegisterAttached(
                @"DocumentXaml",
                typeof (string),
                typeof (RichTextBoxHelper),
                new FrameworkPropertyMetadata
                {
                    BindsTwoWayByDefault = true,
                    PropertyChangedCallback = (obj, e) =>
                    {
                        var richTextBox = (RichTextBox) obj;
                        var xaml = GetDocumentXaml(richTextBox);
                        var doc = new FlowDocument();
                        var range = new TextRange(doc.ContentStart, doc.ContentEnd);
                        range.Load(new MemoryStream(Encoding.UTF8.GetBytes(xaml)),
                            DataFormats.Text);
                        richTextBox.Document = doc;
                        range.Changed += (obj2, e2) =>
                        {
                            if (Equals(richTextBox.Document, doc))
                            {
                                MemoryStream buffer = new MemoryStream();
                                range.Save(buffer, DataFormats.Text);
                                SetDocumentXaml(richTextBox,
                                    Encoding.UTF8.GetString(buffer.ToArray()));
                            }
                        };
                    }
                });
    }
}
