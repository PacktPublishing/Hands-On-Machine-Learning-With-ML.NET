using System;
using System.Threading.Tasks;

using Windows.System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

using chapter10.lib.Enums;

using chapter10_app.ViewModels;

namespace chapter10_app
{
    public sealed partial class MainPage : Page
    {
        private MainPageViewModel ViewModel => (MainPageViewModel) DataContext;

        public MainPage()
        {
            InitializeComponent();

            DataContext = new MainPageViewModel();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            var initialization = ViewModel.Initialize();

            if (initialization)
            {
                return;
            }

            await ShowMessage("Failed to initialize model - verify the model has been created");

            Application.Current.Exit();

            base.OnNavigatedTo(e);
        }

        public async Task<IUICommand> ShowMessage(string message)
        {
            var dialog = new MessageDialog(message);

            return await dialog.ShowAsync();
        }

        private void BtnGo_Click(object sender, RoutedEventArgs e) => Navigate();


        private void Navigate()
        {
            wvMain.Navigate(ViewModel.BuildUri());
        }

        private void TxtBxUrl_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter && ViewModel.EnableGoButton)
            {
                Navigate();
            }
        }

        private void WvMain_OnNavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            if (args.Uri == null)
            {
                return;
            }

            var (classificationResult, browserContent) = ViewModel.Classify(args.Uri.ToString());

            switch (classificationResult)
            {
                case Classification.BENIGN:
                    return;
                case Classification.MALICIOUS:
                    sender.NavigateToString(browserContent);
                    break;
            }
        }
    }
}