using System;
using System.IO;

using chapter07.ML.Base;
using chapter07.ML.Objects;

using Microsoft.ML;
using Microsoft.ML.Trainers;

namespace chapter07.ML
{
    public class Trainer : BaseML
    {
        private const string UserIDEncoding = "UserIDEncoding";

        private const string MusicIDEncoding = "MusicIDEncoding";

        private (IDataView DataView, IEstimator<ITransformer> Transformer) GetDataView(string fileName, bool training = true)
        {
            var trainingDataView = MlContext.Data.LoadFromTextFile<MusicRating>(fileName, ',');

            if (!training)
            {
                return (trainingDataView, null);
            }

            IEstimator<ITransformer> dataProcessPipeline =
                MlContext.Transforms.Conversion.MapValueToKey(outputColumnName: UserIDEncoding,
                        inputColumnName: nameof(MusicRating.UserID))
                    .Append(MlContext.Transforms.Conversion.MapValueToKey(outputColumnName: MusicIDEncoding,
                        inputColumnName: nameof(MusicRating.MusicID)));

            return (trainingDataView, dataProcessPipeline);
        }

        public void Train(string trainingFileName, string testingFileName)
        {
            if (!File.Exists(trainingFileName))
            {
                Console.WriteLine($"Failed to find training data file ({trainingFileName}");

                return;
            }

            if (!File.Exists(testingFileName))
            {
                Console.WriteLine($"Failed to find test data file ({testingFileName}");

                return;
            }

            var trainingDataView = GetDataView(trainingFileName);

            var options = new MatrixFactorizationTrainer.Options
            {
                MatrixColumnIndexColumnName = UserIDEncoding,
                MatrixRowIndexColumnName = MusicIDEncoding,
                LabelColumnName = "Label",
                NumberOfIterations = 20,
                ApproximationRank = 10,
                Quiet = false
            };

            var trainingPipeLine = trainingDataView.Transformer.Append(MlContext.Recommendation().Trainers.MatrixFactorization(options));

            ITransformer trainedModel = trainingPipeLine.Fit(trainingDataView.DataView);

            MlContext.Model.Save(trainedModel, trainingDataView.DataView.Schema, ModelPath);

            Console.WriteLine($"Model saved to {ModelPath}{Environment.NewLine}");

            var testingDataView = GetDataView(testingFileName, true);

            var testSetTransform = trainedModel.Transform(testingDataView.DataView);

            var modelMetrics = MlContext.Recommendation().Evaluate(testSetTransform);

            Console.WriteLine($"Matrix Factorization Evaluation:{Environment.NewLine}{Environment.NewLine}" +
                              $"Loss Function: {modelMetrics.LossFunction:F3}{Environment.NewLine}" +
                              $"Mean Absolute Error: {modelMetrics.MeanAbsoluteError:F3}{Environment.NewLine}" +
                              $"Mean Squared Error: {modelMetrics.MeanSquaredError:F3}{Environment.NewLine}" +
                              $"R Squared: {modelMetrics.RSquared:F3}{Environment.NewLine}" +
                              $"Root Mean Squared Error: {modelMetrics.RootMeanSquaredError:F3}");
        }
    }
}