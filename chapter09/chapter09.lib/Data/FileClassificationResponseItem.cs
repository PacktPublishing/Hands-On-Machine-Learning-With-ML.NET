using System;

using chapter09.lib.Helpers;

namespace chapter09.lib.Data
{
    public class FileClassificationResponseItem
    {
        private const float TRUE = 1.0f;
        private const float FALSE = 0.0f;

        public string SHA1Sum { get; set; }

        public double Confidence { get; set; }

        public bool IsMalicious { get; set; }

        public float FileSize { get; set; }

        public float Is64Bit { get; set; }

        public float NumImports { get; set; }

        public float NumImportFunctions { get; set; }

        public float NumExportFunctions { get; set; }

        public float IsSigned { get; set; }

        public string Strings { get; set; }

        public string ErrorMessage { get; set; }

        public FileClassificationResponseItem()
        {
        }

        public FileClassificationResponseItem(byte[] fileBytes)
        {
            SHA1Sum = fileBytes.ToSHA1();
            Confidence = 0.0;
            IsMalicious = false;
            FileSize = fileBytes.Length;

            try
            {
                var peFile = new PeNet.PeFile(fileBytes);

                Is64Bit = peFile.Is64Bit ? TRUE : FALSE;

                try
                {
                    NumImports = peFile.ImageImportDescriptors.Length;
                }
                catch
                {
                    NumImports = 0.0f;
                }

                NumImportFunctions = peFile.ImportedFunctions.Length;

                if (peFile.ExportedFunctions != null)
                {
                    NumExportFunctions = peFile.ExportedFunctions.Length;
                }

                IsSigned = peFile.IsSigned ? TRUE : FALSE;

                Strings = fileBytes.ToStringsExtraction();
            }
            catch (Exception)
            {
                ErrorMessage = $"Invalid file ({SHA1Sum}) - only PE files are supported";
            }
        }
    }
}