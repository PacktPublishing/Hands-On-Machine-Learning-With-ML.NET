using System;
using System.Linq;

using chapter05.Enums;

using Microsoft.ML.Data;

namespace chapter05.ML.Objects
{
    public class FileData
    {
        private const float TRUE = 1.0f;
        private const float FALSE = 0.0f;

        public FileData(Span<byte> data, string fileName = null)
        {
            // Used for training purposes only
            if (!string.IsNullOrEmpty(fileName))
            {
                if (fileName.Contains("ps1"))
                {
                    Label = (float) FileTypes.Script;
                } else if (fileName.Contains("exe"))
                {
                    Label = (float) FileTypes.Executable;
                } else if (fileName.Contains("doc"))
                {
                    Label = (float) FileTypes.Document;
                }
            }

            IsBinary = HasBinaryContent(data) ? TRUE : FALSE;

            IsMZHeader = HasHeaderBytes(data.Slice(0, 2), "MZ") ? TRUE : FALSE;

            IsPKHeader = HasHeaderBytes(data.Slice(0, 2), "PK") ? TRUE : FALSE;
        }

        /// <summary>
        /// Used for mapping cluster ids to results only
        /// </summary>
        /// <param name="fileType"></param>
        public FileData(FileTypes fileType)
        {
            Label = (float)fileType;

            switch (fileType)
            {
                case FileTypes.Document:
                    IsBinary = TRUE;
                    IsMZHeader = FALSE;
                    IsPKHeader = TRUE;
                    break;
                case FileTypes.Executable:
                    IsBinary = TRUE;
                    IsMZHeader = TRUE;
                    IsPKHeader = FALSE;
                    break;
                case FileTypes.Script:
                    IsBinary = FALSE;
                    IsMZHeader = FALSE;
                    IsPKHeader = FALSE;
                    break;
            }
        }

        private static bool HasBinaryContent(Span<byte> fileContent) =>
            System.Text.Encoding.UTF8.GetString(fileContent.ToArray()).Any(a => char.IsControl(a) && a != '\r' && a != '\n');

        private static bool HasHeaderBytes(Span<byte> data, string match) => System.Text.Encoding.UTF8.GetString(data) == match;

        [ColumnName("Label")]
        public float Label { get; set; }

        public float IsBinary { get; set; }

        public float IsMZHeader { get; set; }

        public float IsPKHeader { get; set; }

        public override string ToString() => $"{Label},{IsBinary},{IsMZHeader},{IsPKHeader}";
    }
}