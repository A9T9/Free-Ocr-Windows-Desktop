using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace a9t9Ocr
{
    public class RightSideViewModel : INotifyPropertyChanged
    {
        public ICommand SaveTextCommand { get; set; }
        public ICommand SaveTextWordCommand { get; set; }
        public ICommand ChangeLanguageCommand { get; set; }

        private string _currentOcrLanguage;
        private ObservableCollection<string> _ocrLanguages;
        public ObservableCollection<string> OcrLanguages
        {
            get { return _ocrLanguages; }
            set
            {
                if (_ocrLanguages == value) return;
                _ocrLanguages = value;
                // ReSharper disable once RedundantArgumentDefaultValue
                OnPropertyChanged("OcrLanguages");
            }
        }
        public string CurrentOcrLanguage
        {
            get { return _currentOcrLanguage; }
            set
            {
                if (_currentOcrLanguage == value)
                    return;
                _currentOcrLanguage = value;
                _tesseractOrc.ChangeLanguage(_currentOcrLanguage);
                // ReSharper disable once RedundantArgumentDefaultValue
                OnPropertyChanged("CurrentOcrLanguage");
            }
        }

        private String _recognizedText;
        public String RecognizedText
        {
            get
            {
                return _recognizedText;
            }
            set
            {
                _recognizedText = value;
                // ReSharper disable once RedundantArgumentDefaultValue
                OnPropertyChanged("RecognizedText");
            }
        }

        public event LanguageChanged LanguageEvent;

        public delegate void LanguageChanged(object sender, EventArgs args);

        const string PushBeginOcr = "---";//begin OCR - add help message here
        private ITesseractOrc _tesseractOrc;
        public RightSideViewModel(ITesseractOrc tesseractOrc)
        {
            _tesseractOrc = tesseractOrc;
            SaveTextCommand = new RelayCommand(SaveText);
            SaveTextWordCommand = new RelayCommand(SaveTextWord);
            ChangeLanguageCommand = new RelayCommand(ChangeLanguage);

            RecognizedText = PushBeginOcr;
        }
        public void ChangeLanguage(object obj)
        {
            if (obj == null) 
                return;
            Thread.CurrentThread.CurrentCulture = new CultureInfo((string)obj);
            OnLanguageEvent(EventArgs.Empty);
        }
        private string SaveTextFileDialog()
        {
            var dlg = new Microsoft.Win32.SaveFileDialog
            {
                FileName = "Text Document",
                DefaultExt = ".txt",
                Filter = "Text documents (.txt)|*.txt"
            };

            var result = dlg.ShowDialog();
            return result != true ? null : dlg.FileName;
        }
        private string SaveWordFileDialog()
        {
            var dlg = new Microsoft.Win32.SaveFileDialog
            {
                FileName = "Word Document",
                DefaultExt = ".docx",
                Filter = "Word documents (.docx)|*.docx"
            };

            var result = dlg.ShowDialog();
            return result != true ? null : dlg.FileName;
        }
        public void SaveText(object obj)
        {
            var path = SaveTextFileDialog();
            if (string.IsNullOrEmpty(path))
                return;

            Task.Factory.StartNew(delegate
            {
                var fileManipulator = new FileManipulator();
                fileManipulator.SaveToFile(path, RecognizedText);
            });
        }
        public void SaveTextWord(object obj)
        {
            var path = SaveWordFileDialog();
            if (string.IsNullOrEmpty(path))
                return;
            Task.Factory.StartNew(delegate
            {
                var fileManipulator = new FileManipulator();
                fileManipulator.SaveToWord(path, RecognizedText);
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnLanguageEvent(EventArgs args)
        {
            var handler = LanguageEvent;
            if (handler != null) handler(this, args);
        }
    }
}
