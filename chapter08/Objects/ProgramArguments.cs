using chapter08.Enums;

namespace chapter08.Objects
{
    public class ProgramArguments
    {
        public ProgramActions Action { get; set; }

        public string TrainingFileName { get; set; }

        public string TestingFileName { get; set; }

        public string PredictionFileName { get; set; }

        public string ModelFileName { get; set; }

        public ProgramArguments()
        {
            ModelFileName = "chapter8.mdl";

            PredictionFileName = @"..\..\..\Data\predict.csv";

            TrainingFileName = @"..\..\..\Data\sampledata.csv";

            TestingFileName = @"..\..\..\Data\testdata.csv";
        }
    }
}