using Microsoft.ML.Data;

namespace chapter13.wpf.ML.Objects
{
    public class ImageDataInputItem
    {
        [LoadColumn(0)]
        public string ImagePath;

        [LoadColumn(1)]
        public string Label;
    }
}