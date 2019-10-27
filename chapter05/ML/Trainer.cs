using System;

using chapter05.ML.Base;
using chapter05.ML.Objects;

using Microsoft.ML;
using Microsoft.ML.Data;

namespace chapter05.ML
{
    public class Trainer : BaseML
    {
        private IDataView GetDataView(string fileName)
        {
            return MlContext.Data.LoadFromTextFile(path: fileName,
                columns: new[]
                {
                    new TextLoader.Column(nameof(FileData.Label), DataKind.Single, 0),
                    new TextLoader.Column(nameof(FileData.IsBinary), DataKind.Single, 1),
                    new TextLoader.Column(nameof(FileData.IsMZHeader), DataKind.Single, 2),
                    new TextLoader.Column(nameof(FileData.IsPKHeader), DataKind.Single, 3)
                },
                hasHeader: false,
                separatorChar: ',');
        }

        public void Train(string trainingFileName, string testingFileName)
        {
            if (!System.IO.File.Exists(trainingFileName))
            {
                Console.WriteLine($"Failed to find training data file ({trainingFileName}");

                return;
            }

            if (!System.IO.File.Exists(testingFileName))
            {
                Console.WriteLine($"Failed to find test data file ({testingFileName}");

                return;
            }

            var trainingDataView = GetDataView(trainingFileName);

            var dataProcessPipeline = MlContext.Transforms.Concatenate(
                FEATURES,
                nameof(FileData.IsBinary),
                nameof(FileData.IsMZHeader),
                nameof(FileData.IsPKHeader));
            
            var trainer = MlContext.Clustering.Trainers.KMeans(featureColumnName: FEATURES, numberOfClusters: 3);
            var trainingPipeline = dataProcessPipeline.Append(trainer);
            var trainedModel = trainingPipeline.Fit(trainingDataView);

            MlContext.Model.Save(trainedModel, trainingDataView.Schema, ModelPath);

            var testingDataView = GetDataView(testingFileName);

            IDataView testDataView = trainedModel.Transform(testingDataView);

            ClusteringMetrics modelMetrics = MlContext.Clustering.Evaluate(
                data: testDataView,
                labelColumnName: "Label",
                scoreColumnName: "Score",
                featureColumnName: FEATURES);

            Console.WriteLine($"Average Distance: {modelMetrics.AverageDistance}");
            Console.WriteLine($"Davies Bould Index: {modelMetrics.DaviesBouldinIndex}");
            Console.WriteLine($"Normalized Mutual Information: {modelMetrics.NormalizedMutualInformation}");
        }
    }
}