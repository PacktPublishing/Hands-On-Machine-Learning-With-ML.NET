using System;
using System.IO;

using chapter06.ML.Base;
using chapter06.ML.Objects;

using Microsoft.ML;
using Microsoft.ML.Transforms.TimeSeries;

namespace chapter06.ML
{
    public class Predictor : BaseML
    {
        public void Predict(string inputDataFile)
        {
            if (!File.Exists(ModelPath))
            {
                Console.WriteLine($"Failed to find model at {ModelPath}");

                return;
            }

            if (!File.Exists(inputDataFile))
            {
                Console.WriteLine($"Failed to find input data at {inputDataFile}");

                return;
            }

            ITransformer mlModel = MlContext.Model.Load(ModelPath, out var modelInputSchema);

            if (mlModel == null)
            {
                Console.WriteLine("Failed to load model");

                return;
            }

            var predictionEngine = mlModel.CreateTimeSeriesEngine<NetworkTrafficHistory, NetworkTrafficPrediction>(MlContext);

            var inputData =
                MlContext.Data.LoadFromTextFile<NetworkTrafficHistory>(inputDataFile, separatorChar: ',');

            var rows = MlContext.Data.CreateEnumerable<NetworkTrafficHistory>(inputData, false);

            Console.WriteLine($"Based on input file ({inputDataFile}):");

            foreach (var row in rows)
            {
                var prediction = predictionEngine.Predict(row);

                Console.Write($"HOST: {row.HostMachine} TIMESTAMP: {row.Timestamp} TRANSFER: {row.BytesTransferred} ");
                Console.Write($"ALERT: {prediction.Prediction[0]} SCORE: {prediction.Prediction[1]:f2} P-VALUE: {prediction.Prediction[2]:F2}{Environment.NewLine}");
            }
        }
    }
}