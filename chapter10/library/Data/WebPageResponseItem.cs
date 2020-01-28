namespace chapter10.lib.Data
{
    public class WebPageResponseItem
    {
        private const float TRUE = 1.0f;
        private const float FALSE = 0.0f;

        public double Confidence { get; set; }

        public bool IsMalicious { get; set; }

        public string Content { get; set; }

        public string ErrorMessage { get; set; }

        public WebPageResponseItem()
        {
        }

        public WebPageResponseItem(string content)
        {
            Content = content;
        }
    }
}