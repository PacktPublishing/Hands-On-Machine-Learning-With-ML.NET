using Microsoft.ML;

namespace chapter13.wpf.ML.Base
{
    public class BaseML
    {
        protected MLContext MlContext;

        public BaseML()
        {
            MlContext = new MLContext(2020);
        }
    }
}