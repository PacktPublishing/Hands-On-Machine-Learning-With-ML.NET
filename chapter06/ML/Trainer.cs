using System;
using System.IO;

using chapter06.Common;
using chapter06.ML.Base;
using chapter06.ML.Objects;

using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;

namespace chapter06.ML
{
    public class Trainer : BaseML
    {
        private (IDataView DataView, IEstimator<ITransformer> Transformer) GetDataView(string fileName, bool training = true)
        {
            var trainingDataView = MlContext.Data.LoadFromTextFile<LoginHistory>(fileName, ',');

            if (!training)
            {
                return (trainingDataView, null);
            }

            IEstimator<ITransformer> dataProcessPipeline = MlContext.Transforms.Concatenate(
                FEATURES, 
                typeof(LoginHistory).ToPropertyList<LoginHistory>(nameof(LoginHistory.Label)));

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

            var options = new RandomizedPcaTrainer.Options
            {
                FeatureColumnName = FEATURES,
                ExampleWeightColumnName = null,
                Rank = 5,
                Oversampling = 20,
                EnsureZeroMean = true,
                Seed = 1
            };

            IEstimator<ITransformer> trainer = MlContext.AnomalyDetection.Trainers.RandomizedPca(options: options);

            EstimatorChain<ITransformer> trainingPipeline = trainingDataView.Transformer.Append(trainer);

            TransformerChain<ITransformer> trainedModel = trainingPipeline.Fit(trainingDataView.DataView);

            MlContext.Model.Save(trainedModel, trainingDataView.DataView.Schema, ModelPath);

            var testingDataView = GetDataView(testingFileName, true);

            var testSetTransform = trainedModel.Transform(testingDataView.DataView);

            var modelMetrics = MlContext.AnomalyDetection.Evaluate(testSetTransform);

            Console.WriteLine($"Area Under Curve: {modelMetrics.AreaUnderRocCurve:P2}{Environment.NewLine}" +
                              $"Detection at FP Count: {modelMetrics.DetectionRateAtFalsePositiveCount}");
        }
    }
}