using Microsoft.ML;

namespace chapter09.lib.ML.Base
{
    public class BaseML
    {
        protected const string FEATURES = "Features";

        protected MLContext MlContext;

        public BaseML()
        {
            MlContext = new MLContext(2020);
        }
    }
}