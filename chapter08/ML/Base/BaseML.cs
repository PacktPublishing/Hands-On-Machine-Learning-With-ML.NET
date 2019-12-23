using Microsoft.ML;

namespace chapter08.ML.Base
{
    public class BaseML
    {
        protected const string FEATURES = "Features";

        protected readonly MLContext MlContext;

        protected BaseML()
        {
            MlContext = new MLContext(2020);
        }
    }
}