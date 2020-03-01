using System;
using System.IO;

using chapter12.wpf.ML.Base;
using chapter12.wpf.ML.Objects;

using Microsoft.ML;

namespace chapter12.wpf.ML
{
    public class ImageClassificationPredictor : BaseML
    {
        // Training Variables
        private static readonly string _assetsPath = Path.Combine(Environment.CurrentDirectory, "assets");
        private static readonly string _imagesFolder = Path.Combine(_assetsPath, "images");
        private readonly string _trainTagsTsv = Path.Combine(_imagesFolder, "tags.tsv");
        private readonly string _inceptionTensorFlowModel = Path.Combine(_assetsPath, "inception", "tensorflow_inception_graph.pb");

        private const string TF_SOFTMAX = "softmax2_pre_activation";
        private const string INPUT = "input";

        private static readonly string ML_NET_MODEL = Path.Combine(Environment.CurrentDirectory, "chapter12.mdl");

        private ITransformer _model;

        private struct InceptionSettings
        {
            public const int ImageHeight = 224;
            public const int ImageWidth = 224;
            public const float Mean = 117;
            public const float Scale = 1;
            public const bool ChannelsLast = true;
        }

        public ImageDataPredictionItem Predict(string filePath) => 
            Predict(new ImageDataInputItem 
                {
                    ImagePath = filePath 
                }
            );

        public (bool Success, string Exception) Initialize()
        {
            try
            {
                if (File.Exists(ML_NET_MODEL))
                {
                    _model = MlContext.Model.Load(ML_NET_MODEL, out DataViewSchema modelSchema);

                    return (true, string.Empty);
                }

                IEstimator<ITransformer> pipeline = MlContext.Transforms.LoadImages(outputColumnName: INPUT, imageFolder: _imagesFolder, inputColumnName: nameof(ImageDataInputItem.ImagePath))
                    .Append(MlContext.Transforms.ResizeImages(outputColumnName: INPUT, imageWidth: InceptionSettings.ImageWidth, imageHeight: InceptionSettings.ImageHeight, inputColumnName: INPUT))
                    .Append(MlContext.Transforms.ExtractPixels(outputColumnName: INPUT, interleavePixelColors: InceptionSettings.ChannelsLast, offsetImage: InceptionSettings.Mean))
                    .Append(MlContext.Model.LoadTensorFlowModel(_inceptionTensorFlowModel)
                    .ScoreTensorFlowModel(outputColumnNames: new[] { TF_SOFTMAX }, inputColumnNames: new[] { INPUT }, addBatchDimensionInput: true))
                    .Append(MlContext.Transforms.Conversion.MapValueToKey(outputColumnName: "LabelKey", inputColumnName: nameof(ImageDataPredictionItem.Label)))
                    .Append(MlContext.MulticlassClassification.Trainers.LbfgsMaximumEntropy(labelColumnName: "LabelKey", featureColumnName: TF_SOFTMAX))
                    .Append(MlContext.Transforms.Conversion.MapKeyToValue(nameof(ImageDataPredictionItem.PredictedLabelValue), "PredictedLabel"))
                    .AppendCacheCheckpoint(MlContext);

                IDataView trainingData = MlContext.Data.LoadFromTextFile<ImageDataInputItem>(path: _trainTagsTsv, hasHeader: false);

                _model = pipeline.Fit(trainingData);

                MlContext.Model.Save(_model, trainingData.Schema, ML_NET_MODEL);

                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                return (false, ex.ToString());
            }
        }

        public ImageDataPredictionItem Predict(ImageDataInputItem image)
        {
            var predictor = MlContext.Model.CreatePredictionEngine<ImageDataInputItem, ImageDataPredictionItem>(_model);

            return predictor.Predict(image);
        }
    }
}