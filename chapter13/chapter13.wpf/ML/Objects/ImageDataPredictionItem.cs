using Microsoft.ML.Data;

namespace chapter13.wpf.ML.Objects
{
    public class ImageNetPrediction
    {
        [ColumnName("grid")]
        public float[] PredictedLabels;
    }
}