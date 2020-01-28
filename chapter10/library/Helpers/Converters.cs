using chapter10.lib.Data;
using chapter10.lib.ML.Objects;

namespace chapter10.lib.Helpers
{
    public static class Converters
    {
        public static WebPageInputItem ToWebPageInputItem(this WebPageResponseItem webPage)
        {
            return new WebPageInputItem
            {
                HTMLContent = webPage.Content
            };
        }
    }
}