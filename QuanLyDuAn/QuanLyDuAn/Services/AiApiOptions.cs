namespace QuanLyDuAn.Services
{
    public class AiApiOptions
    {
        public const string SectionName = "AiApi";

        public string BaseUrl { get; set; } = "http://127.0.0.1:8001";

        public int TimeoutSeconds { get; set; } = 10;

        public int RetryCount { get; set; } = 1;

        public int RetryDelayMilliseconds { get; set; } = 300;
    }
}
