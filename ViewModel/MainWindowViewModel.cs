using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace a9t9Ocr
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public ICommand CloseWindowCommand { get; set; }
        public ICommand MaximizeWindowCommand { get; set; }
        public ICommand MinimizeWindowCommand { get; set; }
        
        private readonly Window _windowForLanguageSupport;
        private readonly RightSideViewModel _rightSideViewModel;
        public MainWindowViewModel(Window windowForLanguageSupport, LeftSideViewModel leftSideViewModel,RightSideViewModel rightSideViewModel)
        {
            _windowForLanguageSupport = windowForLanguageSupport;
            _rightSideViewModel = rightSideViewModel;
            leftSideViewModel.RecoginedEvent += _leftSideViewModel_RecoginedEvent;
            _rightSideViewModel.LanguageEvent += _rightSideViewModel_LanguageEvent;
            leftSideViewModel.ExitEvent += LeftSideViewModelOnExitEvent;
            ChangeLanguage();


            //var cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
            //var culturesName = cultures.Select(cultureInfo => cultureInfo.ThreeLetterWindowsLanguageName).ToList();
            //var uniqueCultures = culturesName.Distinct().ToArray();
            var filesName = new List<string>();
            var path = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + @"\(a9t9)FreeOcr\tessdata";
            if (Directory.Exists(path))
            {
                string[] filePaths = Directory.GetFiles(path, @"*.traineddata");
                filesName.AddRange(filePaths.Select(f => new FileInfo(f)).Select(fi => fi.Name.Split('.')[0]));
                _rightSideViewModel.OcrLanguages = new ObservableCollection<string>(filesName);
            }
            else 
                _rightSideViewModel.OcrLanguages = new ObservableCollection<string>(new List<string>());

            _rightSideViewModel.CurrentOcrLanguage = "eng";

            CloseWindowCommand = new RelayCommand(CloseWindow);
            MaximizeWindowCommand = new RelayCommand(MaximizeWindow);
            MinimizeWindowCommand = new RelayCommand(MinimizeWindow);
        }

        private void LeftSideViewModelOnExitEvent(object sender, EventArgs args)
        {
            CloseWindow(null);
        }

        public void CloseWindow(object obj)
        {
            Properties.Settings.Default.Height = (int)_windowForLanguageSupport.Height;
            Properties.Settings.Default.Width = (int)_windowForLanguageSupport.Width;
            Properties.Settings.Default.Save();
            Environment.Exit(0);
        }
        public void MaximizeWindow(object obj)
        {
            var currentState = _windowForLanguageSupport.WindowState;
            currentState = currentState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
            _windowForLanguageSupport.WindowState = currentState;
        }
        public void MinimizeWindow(object obj)
        {
            _windowForLanguageSupport.WindowState = WindowState.Minimized;
        }
        void _rightSideViewModel_LanguageEvent(object sender, EventArgs args)
        {
            ChangeLanguage();
        }

        void _leftSideViewModel_RecoginedEvent(object sender, RecognizedTextEventArgs args)
        {
            _rightSideViewModel.RecognizedText = args.ChangedText;
        }

        private void ChangeLanguage()
        {
            ResourceDictionary dict = new ResourceDictionary();
            switch (Thread.CurrentThread.CurrentCulture.ToString())
            {
                case "en-US":
                    dict.Source = new Uri(@"..\Resources\ResourceEnglish.xaml", UriKind.Relative);
                    break;
                case "ru-RU":
                    dict.Source = new Uri(@"..\Resources\ResourceRussian.xaml", UriKind.Relative);
                    break;
                default:
                    dict.Source = new Uri(@"..\Resources\ResourceEnglish.xaml", UriKind.Relative);
                    break;
            }
            _windowForLanguageSupport.Resources.MergedDictionaries.Add(dict);
        }

        #region PropertyChangedEventHandler

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

    }
}
