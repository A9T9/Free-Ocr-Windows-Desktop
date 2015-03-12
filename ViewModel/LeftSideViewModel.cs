using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using GongSolutions.Wpf.DragDrop;

namespace a9t9Ocr
{
    public class LeftSideViewModel : INotifyPropertyChanged, IDropTarget
    {
        #region Commands
        public ICommand OpenImageCommand { get; set; }
        public ICommand OpenPdfCommand { get; set; }
        public ICommand BeginOcrCommand { get; set; }
        public ICommand NextImageCommand { get; set; }
        public ICommand PrevImageCommand { get; set; }
        public ICommand ExitCommand { get; set; }
        public ICommand OpenWebSiteCommand { get; set; }
        public ICommand AboutCommand { get; set; }
        public ICommand InstallLanguageCommand { get; set; }
        public ICommand OpenLanguageFolderCommand { get; set; }
        #endregion

        #region Properties
        private List<ImageClass> _imagesList;
        public List<ImageClass> ImagesList
        {
            get { return _imagesList; }
            set
            {
                _imagesList = value;
                OnPropertyChanged();
            }
        }

        private int _currentImageNumber;
        public int CurrentImageNumber
        {
            get
            {
                if (_imagesList.Count != 0)
                    CurrentImage = _imagesList[_currentImageNumber];
                return _currentImageNumber;
            }
            set { _currentImageNumber = value; }
        }

        public string CurrentImageText
        {
            get { return string.Format("Image {0} of {1}", _currentImageNumber + 1, _imagesList.Count); }
        }

        private ImageClass _currentImage;
        public ImageClass CurrentImage
        {
            get
            {
                return _currentImage;
            }
            set
            {
                _currentImage = value;
                // ReSharper disable once RedundantArgumentDefaultValue
                OnPropertyChanged("CurrentImage");
                // ReSharper disable once ExplicitCallerInfoArgument
                OnPropertyChanged("CurrentImageText");
            }
        }

        private bool _isRecognizeAll;
        public bool IsRecognizeAll
        {
            get
            {
                return _isRecognizeAll;
            }
            set
            {
                if (_isRecognizeAll == value)
                    return;
                _isRecognizeAll = value;
                // ReSharper disable once RedundantArgumentDefaultValue
                OnPropertyChanged("IsRecognizeAll");
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

        public event RecognizedTextChanged RecoginedEvent;

        #endregion

        public event ExitEventChanged ExitEvent;

        public delegate void ExitEventChanged(object sender, EventArgs args);

        private readonly ITesseractOrc _tesseractOrc;
        private readonly string _pathToTestData = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + @"\(a9t9)FreeOcr\tessdata"; 
        readonly ImageConverter _converter = new ImageConverter();
        public LeftSideViewModel(ITesseractOrc orc)
        {
            _tesseractOrc = orc;
            OpenImageCommand = new RelayCommand(OpenImages);
            OpenPdfCommand = new RelayCommand(OpenPdf);
            BeginOcrCommand = new RelayCommand(BeginOcr);
            NextImageCommand = new RelayCommand(NextImage);
            PrevImageCommand = new RelayCommand(PrevImage);

            ExitCommand = new RelayCommand(Exit);
            OpenWebSiteCommand = new RelayCommand(OpenWebSite);
            AboutCommand = new RelayCommand(About);
            InstallLanguageCommand = new RelayCommand(InstallLanguage);
            OpenLanguageFolderCommand = new RelayCommand(OpenLanguageFolder);

            ImagesList = new List<ImageClass>();

            CurrentImage = new ImageClass
            {
                Image = new BitmapImage(new Uri(@"pack://application:,,,/(a9t9)OcrDesktop;component/introtext.jpg",
                        UriKind.Absolute)),
                FilePath = "introtext.jpg"//otherwise image not found, also: file has to be in same folder as EXE
            };
            ImagesList.Add(CurrentImage);
        }
        public void IncrementCurrentImage()
        {
            CurrentImageNumber++;
            if (_currentImageNumber >= _imagesList.Count)
                CurrentImageNumber = 0;
            // ReSharper disable once ExplicitCallerInfoArgument
            OnPropertyChanged("CurrentImageNumber");
        }
        public void DecrementCurrentImage()
        {
            CurrentImageNumber--;
            if (_currentImageNumber < 0)
                CurrentImageNumber = _imagesList.Count - 1;
            // ReSharper disable once ExplicitCallerInfoArgument
            OnPropertyChanged("CurrentImageNumber");
        }
        public void NextImage(object obj)
        {
            if (_imagesList.Count == 0)
                return;
            IncrementCurrentImage();
        }
        public void PrevImage(object obj)
        {
            if (_imagesList.Count == 0)
                return;
            DecrementCurrentImage();
        }
        public void OpenImages(object obj)
        {
            try
            {
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
                {
                    Multiselect = true,
                };
                bool? result = dlg.ShowDialog();
                if (result != true)
                    return;

                var filenames = dlg.FileNames;

                ImagesList.Clear();

                var imageLoader = new ImageLoader();
                ImagesList.AddRange(imageLoader.LoadImages(filenames.ToList()));

                CurrentImageNumber = 0;

                NextImage(obj);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace);
            }
        }
        public void BeginOcr(object obj)
        {
            if (!Directory.Exists(_pathToTestData))
            {
                MessageBox.Show("I am speechless: No Tesseract language data found.");
                return;
            }


            if (_imagesList.Count == 0)
                return;
            
            Task.Factory.StartNew(delegate
            {
                try
                {
                    RecognizedText = @"OCR started... ";
                    OnRecoginedEvent(_recognizedText);

                    var recognizedText = new List<string>();
                    if (IsRecognizeAll)
                    {
                        //var tempText = _tesseractOrc.RecognizeFewImages(_imagesList);
                        //recognizedText.AddRange(tempText.Where(text => text != null));
                        int cnt = 1;
                        foreach (var imageClass in _imagesList)
                        {
                            OnRecoginedEvent(String.Format("Image {0} of {1}", cnt++, _imagesList.Count));
                            var tt = _tesseractOrc.RecognizeOneImage(imageClass);
                            recognizedText.Add(tt);
                        }
                    }
                    else
                    {
                        var tempText = _tesseractOrc.RecognizeOneImage(_currentImage);
                        if (tempText == null)
                            return;
                    
                        recognizedText.Add(tempText);
                    }

                    _recognizedText = string.Empty;
                    foreach (var text in recognizedText)
                    {
                        _recognizedText = string.Concat(_recognizedText, text);
                    }
                    RecognizedText = _recognizedText;
                    OnRecoginedEvent(_recognizedText);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.StackTrace);
                }
            });
        }

        public void OpenPdf(object obj)
        {
            try
            {
                if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + @"\gs") &&
                    !Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + @"\gs"))
                {
                    MessageBox.Show("Missing Ghost Script");
                    return;
                }


                var path = OpenPdfFileDialog();
                if (string.IsNullOrEmpty(path))
                    return;

                var fileNames = _converter.PathToConvertedImages(path);
                ImagesList.Clear();

                var imageLoader = new ImageLoader();
                ImagesList.AddRange(imageLoader.LoadImages(fileNames.ToList()));

                NextImage(obj);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.StackTrace);
            }
        }

        private string OpenPdfFileDialog()
        {
            var dlg = new Microsoft.Win32.OpenFileDialog()
            {
                FileName = "Pdf Document",
                DefaultExt = ".pdf",
                Filter = "Pdf documents (.pdf)|*.pdf"
            };

            var result = dlg.ShowDialog();
            return result != true ? null : dlg.FileName;
        }

        void IDropTarget.DragOver(IDropInfo dropInfo)
        {
            dropInfo.Effects = DragDropEffects.Copy;
        }
        void IDropTarget.Drop(IDropInfo dropInfo)
        {
            var dragFileList = ((DataObject)dropInfo.Data).GetFileDropList().Cast<string>();
            dropInfo.Effects = DragDropEffects.Copy;

            var fileList = dragFileList as string[] ?? dragFileList.ToArray();
            var fi = new FileInfo(fileList[0]);
            if (fi.Extension == ".pdf")
            {
                if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + @"\gs") &&
                    !Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + @"\gs"))
                {
                    MessageBox.Show("Missing Ghost Script");
                    return;
                }

                var fileNames = _converter.PathToConvertedImages(fileList[0]);
                ImagesList.Clear();

                var loader = new ImageLoader();
                ImagesList.AddRange(loader.LoadImages(fileNames.ToList()));

                NextImage(null);

                return;
            }
            var imageLoader = new ImageLoader();
            ImagesList.Clear();
            ImagesList.AddRange(imageLoader.LoadImages(fileList.ToList()));
            NextImage(null);
        }

        private void InstallLanguage(object obj)
        {
            System.Diagnostics.Process.Start("http://blog.a9t9.com/p/free-ocr-windows.html#add");
        }
        private void OpenLanguageFolder(object obj)
        {
            try
            {
                var path = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + @"\(a9t9)FreeOcr\tessdata";
                if (!Directory.Exists(path))
                {
                    MessageBox.Show("Folder user documents >(a9t9)FreeOcr\tessdata for Tesseract language data not found.");
                    return;
                }
                //    Directory.CreateDirectory(path);
                System.Diagnostics.Process.Start(path);
            }
            catch (Exception)
            {
                // ignored
            }
        }
        private void Exit(object obj)
        {
            OnExitEvent();
        }
        private void OpenWebSite(object obj)
        {
            System.Diagnostics.Process.Start("http://blog.a9t9.com/p/free-ocr-windows.html");
        }
        private void About(object obj)
        {
            MessageBox.Show("(a9t9) Free OCR for Windows Desktop V1.08\n\n(c) 2015 http://a9t9.com - Full source code available (GPL)\n\nCredits:\n* PDF processing: Ghostscript (GPL)\n* Iconset Dortmund (Creative Commons 3.0)", "About (a9t9) Free OCR for Windows Desktop");
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        protected virtual void OnExitEvent()
        {
            var handler = ExitEvent;
            if (handler != null)
            {
                var ev = new EventArgs();
                handler(this, ev);
            }
        }
        protected virtual void OnRecoginedEvent(string text)
        {
            var handler = RecoginedEvent;
            if (handler != null)
            {
                var ev = new RecognizedTextEventArgs { ChangedText = text };
                handler(this, ev);
            }
        }
    }

    public delegate void RecognizedTextChanged(object sender, RecognizedTextEventArgs args);

    public class RecognizedTextEventArgs : EventArgs
    {
        public string ChangedText;
    }
}
