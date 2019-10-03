using Microsoft.ML.Data;

namespace chapter02.ML.Objects
{
    public class RestaurantFeedback
    {
        [LoadColumn(0)]
        public bool Label { get; set; }

        [LoadColumn(1)]
        public string Text { get; set; }
    }
}