using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;

using chapter13.wpf.ML.Base;
using chapter13.wpf.ML.Objects;
using chapter13.wpf.ML.Yolo;

using Microsoft.ML;
using Microsoft.ML.Data;

namespace chapter13.wpf.ML
{
    public class ImageClassificationPredictor : BaseML
    {
        private static readonly string ML_NET_MODEL = Path.Combine(Environment.CurrentDirectory, "assets/model", "TinyYolo2_model.onnx");

        private ITransformer _model;

        public struct ImageNetSettings
        {
            public const int imageHeight = 416;
            public const int imageWidth = 416;
        }

        public struct TinyYoloModelSettings
        {
            public const string ModelInput = "image";

            public const string ModelOutput = "grid";
        }

        public (bool Success, string Exception) Initialize()
        {
            try
            {
                if (File.Exists(ML_NET_MODEL))
                {
                    var data = MlContext.Data.LoadFromEnumerable(new List<ImageDataInputItem>());

                    var pipeline = MlContext.Transforms.LoadImages(outputColumnName: "image", imageFolder: "", 
                            inputColumnName: nameof(ImageDataInputItem.ImagePath))
                        .Append(MlContext.Transforms.ResizeImages(outputColumnName: "image", 
                            imageWidth: ImageNetSettings.imageWidth, 
                            imageHeight: ImageNetSettings.imageHeight, 
                            inputColumnName: "image"))
                        .Append(MlContext.Transforms.ExtractPixels(outputColumnName: "image"))
                        .Append(MlContext.Transforms.ApplyOnnxModel(modelFile: ML_NET_MODEL, 
                            outputColumnNames: new[] { TinyYoloModelSettings.ModelOutput }, 
                            inputColumnNames: new[] { TinyYoloModelSettings.ModelInput }));

                    _model = pipeline.Fit(data);

                    return (true, string.Empty);
                }

                return (false, string.Empty);
            }
            catch (Exception ex)
            {
                return (false, ex.ToString());
            }
        }

        private static byte[] DrawBoundingBox(string fileName, IList<YoloBoundingBox> filteredBoundingBoxes)
        {
            var image = Image.FromFile(fileName);

            var originalImageHeight = image.Height;
            var originalImageWidth = image.Width;

            foreach (var box in filteredBoundingBoxes)
            {
                var x = (uint)Math.Max(box.Dimensions.X, 0);
                var y = (uint)Math.Max(box.Dimensions.Y, 0);
                var width = (uint)Math.Min(originalImageWidth - x, box.Dimensions.Width);
                var height = (uint)Math.Min(originalImageHeight - y, box.Dimensions.Height);

                x = (uint)originalImageWidth * x / ImageNetSettings.imageWidth;
                y = (uint)originalImageHeight * y / ImageNetSettings.imageHeight;
                width = (uint)originalImageWidth * width / ImageNetSettings.imageWidth;
                height = (uint)originalImageHeight * height / ImageNetSettings.imageHeight;

                var text = $"{box.Label} ({box.Confidence * 100:0}%)";

                using (var thumbnailGraphic = Graphics.FromImage(image))
                {
                    thumbnailGraphic.CompositingQuality = CompositingQuality.HighQuality;
                    thumbnailGraphic.SmoothingMode = SmoothingMode.HighQuality;
                    thumbnailGraphic.InterpolationMode = InterpolationMode.HighQualityBicubic;

                    var drawFont = new Font("Arial", 20, FontStyle.Bold);
                    var size = thumbnailGraphic.MeasureString(text, drawFont);
                    var fontBrush = new SolidBrush(Color.Black);
                    var atPoint = new Point((int)x, (int)y - (int)size.Height - 1);

                    var pen = new Pen(box.BoxColor, 3.2f);
                    var colorBrush = new SolidBrush(box.BoxColor);

                    thumbnailGraphic.FillRectangle(colorBrush, (int)x, (int)(y - size.Height - 1), (int)size.Width, (int)size.Height);
                    thumbnailGraphic.DrawString(text, drawFont, fontBrush, atPoint);

                    thumbnailGraphic.DrawRectangle(pen, x, y, width, height);
                }
            }

            using (var ms = new MemoryStream())
            {
                image.Save(ms, image.RawFormat);

                return ms.ToArray();
            }
        }

        public byte[] Predict(string fileName)
        {
            var imageDataView = MlContext.Data.LoadFromEnumerable(new List<ImageDataInputItem> { new ImageDataInputItem { ImagePath = fileName } });

            var scoredData = _model.Transform(imageDataView);

            var probabilities = scoredData.GetColumn<float[]>(TinyYoloModelSettings.ModelOutput);

            var parser = new YoloOutputParser();

            var boundingBoxes =
                probabilities
                    .Select(probability => parser.ParseOutputs(probability))
                    .Select(boxes => parser.FilterBoundingBoxes(boxes, 5, .5F));

            return DrawBoundingBox(fileName, boundingBoxes.FirstOrDefault());
        }
    }
}