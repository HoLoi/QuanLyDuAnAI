namespace QuanLyDuAn.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendAsync(string toEmail, string subject, string body);
        Task SendAccountActivationEmailAsync(string toEmail, string recipientName, string userName, string activationUrl, int lifetimeHours);
    }
}
