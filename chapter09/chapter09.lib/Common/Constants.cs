using System;
using System.IO;

namespace chapter09.lib.Common
{
    public static class Constants
    {
        public static string MODEL_PATH = Path.Combine(AppContext.BaseDirectory, "fileclassification.mdl");

        public const string SAMPLE_DATA = "sampledata.data";

        public const string TEST_DATA = "testdata.data";
    }
}