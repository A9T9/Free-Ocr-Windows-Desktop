using System.Windows;
using System.Windows.Input;

namespace a9t9Ocr
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        //I'm a newbie in mvvm, so i don't know how to move this to mvvm model :)
        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            Height = Properties.Settings.Default.Height;
            Width = Properties.Settings.Default.Width;

            var tess = new TesseractOcr("eng");
            var leftVm = new LeftSideViewModel(tess);
            LeftSideControl.DataContext = leftVm;

            var rightVm = new RightSideViewModel(tess);
            RightSideControl.DataContext = rightVm;

            var view = new MainWindowViewModel(this, leftVm, rightVm);
            DataContext = view;

            MenuItemFile.DataContext = leftVm;
            MenuItemHelp.DataContext = leftVm;
            MenuItemSave.DataContext = rightVm;
  //not yet    MenuItemLanguage.DataContext = rightVm;
        }
        private void WindowMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
