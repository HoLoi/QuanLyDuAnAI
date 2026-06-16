namespace QuanLyDuAn.Services
{
    public sealed class AccountActivationOptions
    {
        public const string SectionName = "AccountActivation";

        public int TokenLifetimeHours { get; set; } = 24;
        public int ResendCooldownSeconds { get; set; } = 60;
        public string AppBaseUrl { get; set; } = string.Empty;
    }
}
