using System;
using System.IO;

using chapter08.ML.Base;
using chapter08.ML.Objects;
using chapter08.Objects;

using Microsoft.ML;
using Microsoft.ML.Transforms.TimeSeries;

namespace chapter08.ML
{
    public class Trainer : BaseML
    {
        public void Train(ProgramArguments arguments)
        {
            if (!File.Exists(arguments.TrainingFileName))
            {
                Console.WriteLine($"Failed to find training data file ({arguments.TrainingFileName})");

                return;
            }

            if (!File.Exists(arguments.TestingFileName))
            {
                Console.WriteLine($"Failed to find test data file ({arguments.TestingFileName})");

                return;
            }

            var dataView = MlContext.Data.LoadFromTextFile<StockPrices>(arguments.TrainingFileName);

            var model = MlContext.Forecasting.ForecastBySsa(
                outputColumnName: nameof(StockPrediction.StockForecast),
                inputColumnName: nameof(StockPrices.StockPrice), 
                windowSize: 7, 
                seriesLength: 30, 
                trainSize: 24, 
                horizon: 5,
                confidenceLevel: 0.95f,
                confidenceLowerBoundColumn: nameof(StockPrediction.LowerBound),
                confidenceUpperBoundColumn: nameof(StockPrediction.UpperBound));

            var transformer = model.Fit(dataView);

            var forecastEngine = transformer.CreateTimeSeriesEngine<StockPrices, StockPrediction>(MlContext);

            forecastEngine.CheckPoint(MlContext, arguments.ModelFileName);

            Console.WriteLine($"Wrote model to {arguments.ModelFileName}");
        }
    }
}