using Microsoft.ML.Data;

namespace chapter03_logistic_regression.ML.Objects
{
    public class FilePrediction
    {
        [ColumnName("PredictedLabel")]
        public bool IsMalicious { get; set; }

        public float Probability { get; set; }

        public float Score { get; set; }
    }
}