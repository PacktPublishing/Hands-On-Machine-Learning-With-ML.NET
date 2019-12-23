using System;
using System.IO;
using System.Linq;

using chapter08.ML.Base;
using chapter08.ML.Objects;
using chapter08.Objects;

using Microsoft.ML;
using Microsoft.ML.Transforms.TimeSeries;

namespace chapter08.ML
{
    public class Predictor : BaseML
    {
        public void Predict(ProgramArguments arguments)
        {
            if (!File.Exists(arguments.ModelFileName))
            {
                Console.WriteLine($"Failed to find model at {arguments.ModelFileName}");

                return;
            }

            if (!File.Exists(arguments.PredictionFileName))
            {
                Console.WriteLine($"Failed to find input data at {arguments.PredictionFileName}");

                return;
            }

            ITransformer mlModel;

            using (var stream = new FileStream(Path.Combine(AppContext.BaseDirectory, arguments.ModelFileName), FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                mlModel = MlContext.Model.Load(stream, out _);
            }

            if (mlModel == null)
            {
                Console.WriteLine("Failed to load model");

                return;
            }

            var predictionEngine = mlModel.CreateTimeSeriesEngine<StockPrices, StockPrediction>(MlContext);

            var stockPrices = File.ReadAllLines(arguments.PredictionFileName);

            foreach (var stockPrice in stockPrices)
            {
                var prediction = predictionEngine.Predict(new StockPrices(Convert.ToSingle(stockPrice)));

                Console.WriteLine($"Given a stock price of ${stockPrice}, the next 5 values are predicted to be: " +
                                  $"{string.Join(", ", prediction.StockForecast.Select(a => $"${Math.Round(a)}"))}");
            }
        }
    }
}