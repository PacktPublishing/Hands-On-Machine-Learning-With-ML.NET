using System;

using chapter08.Enums;
using chapter08.Helpers;
using chapter08.ML;
using chapter08.Objects;

namespace chapter08
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.Clear();

            var arguments = CommandLineParser.ParseArguments<ProgramArguments>(args);

            switch (arguments.Action)
            {
                case ProgramActions.PREDICT:
                    new Predictor().Predict(arguments);
                    break;
                case ProgramActions.TRAINING:
                    new Trainer().Train(arguments);
                    break;
                default:
                    Console.WriteLine($"Unhandled action {arguments.Action}");
                    break;
            }
        }
    }
}