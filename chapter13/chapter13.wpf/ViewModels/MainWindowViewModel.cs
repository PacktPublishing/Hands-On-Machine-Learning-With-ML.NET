using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using chapter13.wpf.ML;

using Microsoft.Win32;

namespace chapter13.wpf.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly ImageClassificationPredictor _prediction = new ImageClassificationPredictor();

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

            Classify(ofd.FileName);
        }

        private void LoadImageBytes(byte[] parsedImageBytes)
        {
            var image = new BitmapImage();

            using (var mem = new MemoryStream(parsedImageBytes))
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

            LoadImageBytes(result);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}