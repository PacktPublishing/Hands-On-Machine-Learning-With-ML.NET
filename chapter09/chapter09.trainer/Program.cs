using System;
using System.Text;

using chapter09.lib.ML;

using chapter09.trainer.Enums;
using chapter09.trainer.Helpers;
using chapter09.trainer.Objects;

namespace chapter09.trainer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            Console.Clear();

            var arguments = CommandLineParser.ParseArguments<ProgramArguments>(args);

            switch (arguments.Action)
            {
                case ProgramActions.FEATURE_EXTRACTOR:
                    new FileClassificationFeatureExtractor().Extract(arguments.TrainingFolderPath,
                        arguments.TestingFolderPath);
                    break;
                case ProgramActions.PREDICT:
                    var prediction = new FileClassificationPredictor().Predict(arguments.PredictionFileName);

                    Console.WriteLine($"File is {(prediction.IsMalicious ? "malicious" : "clean")} with a {prediction.Confidence:P2}% confidence");
                    break;
                case ProgramActions.TRAINING:
                    new FileClassificationTrainer().Train(arguments.TrainingFileName, arguments.TestingFileName);
                    break;
                default:
                    Console.WriteLine($"Unhandled action {arguments.Action}");
                    break;
            }
        }
    }
}