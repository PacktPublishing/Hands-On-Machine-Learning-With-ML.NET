using chapter12.wpf.ViewModels;

using System.Windows;

namespace chapter12.wpf
{
    public partial class MainWindow : Window
    {
        private MainWindowViewModel ViewModel => (MainWindowViewModel)DataContext;

        public MainWindow()
        {
            InitializeComponent();

            DataContext = new MainWindowViewModel();

            var (success, exception) = ViewModel.Initialize();

            if (success)
            {
                return;
            }

            MessageBox.Show($"Failed to initialize model - {exception}");

            Application.Current.Shutdown();
        }

        private void btnSelectFile_Click(object sender, RoutedEventArgs e) => ViewModel.SelectFile();
    }
}