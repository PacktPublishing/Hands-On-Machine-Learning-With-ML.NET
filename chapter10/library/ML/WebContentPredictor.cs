using System.Reflection;

using chapter10.lib.Common;
using chapter10.lib.Data;
using chapter10.lib.Helpers;
using chapter10.lib.ML.Base;
using chapter10.lib.ML.Objects;

using Microsoft.ML;

namespace chapter10.lib.ML
{
    public class WebContentPredictor : BaseML
    {
        private ITransformer _model;

        public WebPageResponseItem Predict(string url) => Predict(new WebPageResponseItem(url.ToWebContentString()));

        public bool Initialize()
        {
            var assembly = typeof(WebContentPredictor).GetTypeInfo().Assembly;

            var resource = assembly.GetManifestResourceStream($"chapter10.lib.Model.{Constants.MODEL_NAME}");

            if (resource == null)
            {
                return false;
            }

            _model = MlContext.Model.Load(resource, out _);

            return true;
        }

        public WebPageResponseItem Predict(WebPageResponseItem webPage)
        {
            var predictionEngine = MlContext.Model.CreatePredictionEngine<WebPageInputItem, WebPagePredictionItem>(_model);

            var prediction = predictionEngine.Predict(webPage.ToWebPageInputItem());

            webPage.Confidence = prediction.Probability;
            webPage.IsMalicious = prediction.Prediction;

            return webPage;
        }
    }
}