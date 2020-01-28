using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using chapter10.lib.Common;
using chapter10.lib.Enums;
using chapter10.lib.ML;

namespace chapter10_app.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        private readonly WebContentPredictor _prediction = new WebContentPredictor();

        private bool _enableGoButton;

        public bool EnableGoButton
        {
            get => _enableGoButton;

            private set
            {
                _enableGoButton = value;
                OnPropertyChanged();
            }
        }

        private string _webServiceURL;

        public string WebServiceURL
        {
            get => _webServiceURL;

            set
            {
                _webServiceURL = value;

                OnPropertyChanged();

                EnableGoButton = !string.IsNullOrEmpty(value);
            }
        }

        private string _webPageClassification;

        public string WebPageClassification
        {
            get => _webPageClassification;

            set
            {
                _webPageClassification = value;
                OnPropertyChanged();
            }
        }

        public bool Initialize() => _prediction.Initialize();

        public Uri BuildUri()
        {
            var webServiceUrl = WebServiceURL;

            if (!webServiceUrl.StartsWith("http://", StringComparison.InvariantCultureIgnoreCase) &&
                !webServiceUrl.StartsWith("https://", StringComparison.InvariantCultureIgnoreCase))
            {
                webServiceUrl = $"http://{webServiceUrl}";
            }

            return new Uri(webServiceUrl);
        }

        public (Classification ClassificationResult, string BrowserContent) Classify(string url)
        {
            var result = _prediction.Predict(url);

            WebPageClassification = $"Webpage is considered {result.Confidence:P1} malicious";

            return result.Confidence < Constants.MALICIOUS_THRESHOLD ? 
                (Classification.BENIGN, string.Empty) : 
                (Classification.MALICIOUS, $"<html><body bgcolor=\"red\"><h2 style=\"text-align: center\">Machine Learning has found {WebServiceURL} to be a malicious site and was blocked automatically</h2></body></html>");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}