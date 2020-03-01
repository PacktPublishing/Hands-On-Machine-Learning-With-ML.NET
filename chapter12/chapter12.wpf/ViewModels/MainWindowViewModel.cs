using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using chapter12.wpf.ML;

using Microsoft.Win32;

namespace chapter12.wpf.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly ImageClassificationPredictor _prediction = new ImageClassificationPredictor();

        private string _imageClassification;

        public string ImageClassification
        {
            get => _imageClassification;

            set
            {
                _imageClassification = value;
                OnPropertyChanged();
            }
        }

        private ImageSource _imageSource;

        public ImageSource SelectedImageSource
        {
            get => _imageSource;

            set
            {
                _imageSource = value;
                OnPropertyChanged();
            }
        }

        public (bool Success, string Exception) Initialize() => _prediction.Initialize();

        public void SelectFile()
        {
            var ofd = new OpenFileDialog
            {
                Filter = "Image Files(*.BMP;*.JPG;*.PNG)|*.BMP;*.JPG;*.PNG"
            };

            var result = ofd.ShowDialog();

            if (!result.HasValue || !result.Value)
            {
                return;
            }

            LoadImageBytes(ofd.FileName);

            Classify(ofd.FileName);
        }

        private void LoadImageBytes(string fileName)
        {
            var image = new BitmapImage();

            var imageData = File.ReadAllBytes(fileName);

            using (var mem = new MemoryStream(imageData))
            {
                mem.Position = 0;

                image.BeginInit();
                
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                
                image.EndInit();
            }

            image.Freeze();

            SelectedImageSource = image;
        }

        public void Classify(string imagePath)
        {
            var result = _prediction.Predict(imagePath);

            ImageClassification = $"Image ({imagePath}) is a picture of {result.PredictedLabelValue} with a confidence of {result.Score.Max().ToString("P2")}";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}