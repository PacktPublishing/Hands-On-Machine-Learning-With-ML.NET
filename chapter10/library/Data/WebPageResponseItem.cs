namespace chapter10.lib.Data
{
    public class WebPageResponseItem
    {
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