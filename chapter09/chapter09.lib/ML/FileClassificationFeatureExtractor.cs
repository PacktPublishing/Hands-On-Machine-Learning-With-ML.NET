using System;
using System.IO;

using chapter09.lib.Common;
using chapter09.lib.Data;
using chapter09.lib.Helpers;

namespace chapter09.lib.ML
{
    public class FileClassificationFeatureExtractor
    {
        private void ExtractFolder(string folderPath, string outputFile)
        {
            if (!Directory.Exists(folderPath))
            {
                Console.WriteLine($"{folderPath} does not exist");

                return;
            }

            var files = Directory.GetFiles(folderPath);

            using (var streamWriter =
                new StreamWriter(Path.Combine(AppContext.BaseDirectory, $"../../../../{outputFile}")))
            {
                foreach (var file in files)
                {
                    var extractedData = new FileClassificationResponseItem(File.ReadAllBytes(file)).ToFileData();

                    extractedData.Label = !file.Contains("clean");

                    streamWriter.WriteLine(extractedData.ToString());
                }
            }

            Console.WriteLine($"Extracted {files.Length} to {outputFile}");
        }

        public void Extract(string trainingPath, string testPath)
        {
            ExtractFolder(trainingPath, Constants.SAMPLE_DATA);
            ExtractFolder(testPath, Constants.TEST_DATA);
        }
    }
}