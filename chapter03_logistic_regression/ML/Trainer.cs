using System;
using System.IO;

using chapter03_logistic_regression.ML.Base;
using chapter03_logistic_regression.ML.Objects;

using Microsoft.ML;

namespace chapter03_logistic_regression.ML
{
    public class Trainer : BaseML
    {
        public void Train(string trainingFileName)
        {
            if (!File.Exists(trainingFileName))
            {
                Console.WriteLine($"Failed to find training data file ({trainingFileName}");

                return;
            }

            var trainingDataView = MlContext.Data.LoadFromTextFile<FileInput>(trainingFileName);

            var dataSplit = MlContext.Data.TrainTestSplit(trainingDataView, testFraction: 0.2);

            var dataProcessPipeline = MlContext.Transforms.CopyColumns("Label", nameof(FileInput.Label))
                .Append(MlContext.Transforms.Text.FeaturizeText("NGrams", nameof(FileInput.Strings)))
                .Append(MlContext.Transforms.Concatenate("Features", "NGrams"));

            var trainer = MlContext.BinaryClassification.Trainers.SdcaLogisticRegression(labelColumnName: "Label", featureColumnName: "Features");

            var trainingPipeline = dataProcessPipeline.Append(trainer);

            ITransformer trainedModel = trainingPipeline.Fit(dataSplit.TrainSet);
            MlContext.Model.Save(trainedModel, dataSplit.TrainSet.Schema, ModelPath);
        }
    }
}