using System;

using chapter05.ML;

namespace chapter05
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine($"Invalid arguments passed in, exiting.{Environment.NewLine}{Environment.NewLine}Usage:{Environment.NewLine}" +
                                  $"predict <path to input file>{Environment.NewLine}" +
                                  $"or {Environment.NewLine}" +
                                  $"train <path to training data file> <path to test data file>{Environment.NewLine}" +
                                  $"or {Environment.NewLine}" + $"extract <path to training folder> <path to test folder>{Environment.NewLine}");

                return;
            }

            switch (args[0])
            {
                case "extract":
                    new FeatureExtractor().Extract(args[1], args[2]);
                    break;
                case "predict":
                    new Predictor().Predict(args[1]);
                    break;
                case "train":
                    new Trainer().Train(args[1], args[2]);
                    break;
                default:
                    Console.WriteLine($"{args[0]} is an invalid option");
                    break;
            }
        }
    }
}