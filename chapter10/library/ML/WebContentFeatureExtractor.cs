using System;
using System.Collections.Generic;
using System.IO;

using chapter10.lib.Helpers;

namespace chapter10.lib.ML
{
    public class WebContentFeatureExtractor
    {
        private static void GetContentFile(string inputFile, string outputFile)
        {
            var lines = File.ReadAllLines(inputFile);

            var urlContent = new List<string>();

            foreach (var line in lines)
            {
                var url = line.Split(',')[0];
                var label = Convert.ToBoolean(line.Split(',')[1]);

                Console.WriteLine($"Attempting to pull HTML from {line}");

                try
                {
                    var content = url.ToWebContentString();

                    content = content.Replace('|', '-');

                    urlContent.Add($"{label}|{content}");
                }
                catch (Exception)
                {
                    Console.WriteLine($"Failed to pull HTTP Content from {url}");
                }
            }

            File.WriteAllText(Path.Combine(AppContext.BaseDirectory, outputFile), string.Join(Environment.NewLine, urlContent));
        }

        public void Extract(string trainingURLList, string testURLList, string trainingOutputFileName, string testingOutputFileName)
        {
            GetContentFile(trainingURLList, trainingOutputFileName);

            GetContentFile(testURLList, testingOutputFileName);
        }
    }
}