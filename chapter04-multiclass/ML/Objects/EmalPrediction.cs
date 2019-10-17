using Microsoft.ML.Data;

namespace chapter04_multiclass.ML.Objects
{
    public class EmalPrediction
    {
        [ColumnName("PredictedLabel")]
        public string Category;
    }
}