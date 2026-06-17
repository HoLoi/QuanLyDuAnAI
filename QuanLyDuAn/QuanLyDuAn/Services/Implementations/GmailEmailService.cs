using System.Net;
using System.Net.Mail;
using System.Text;
using Microsoft.Extensions.Logging;
using QuanLyDuAn.Services.Interfaces;

namespace QuanLyDuAn.Services.Implementations
{
    public class GmailEmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<GmailEmailService> _logger;

        public GmailEmailService(
            IConfiguration configuration,
            ILogger<GmailEmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendAsync(string toEmail, string subject, string body)
        {
            var smtpServer = (_configuration["EmailSettings:SmtpServer"] ?? "smtp.gmail.com").Trim();
            var port = int.TryParse(_configuration["EmailSettings:Port"], out var p) ? p : 587;
            var senderEmail = _configuration["EmailSettings:SenderEmail"]?.Trim();
            var senderName = _configuration["EmailSettings:SenderName"] ?? "QuanLyDuAn AI";
            var username = _configuration["EmailSettings:Username"]?.Trim();
            var appPassword = _configuration["EmailSettings:AppPassword"]?
                .Replace(" ", string.Empty, StringComparison.Ordinal)
                .Trim();

            if (string.IsNullOrWhiteSpace(smtpServer))
            {
                throw new InvalidOperationException("Cấu hình EmailSettings:SmtpServer chưa hợp lệ.");
            }

            if (port <= 0 || port > 65535)
            {
                throw new InvalidOperationException("Cấu hình EmailSettings:Port chưa hợp lệ.");
            }

            if (string.IsNullOrWhiteSpace(senderEmail))
            {
                throw new InvalidOperationException("Cấu hình EmailSettings:SenderEmail chưa đầy đủ.");
            }

            if (string.IsNullOrWhiteSpace(username))
            {
                throw new InvalidOperationException("Cấu hình EmailSettings:Username chưa đầy đủ.");
            }

            if (string.IsNullOrWhiteSpace(appPassword))
            {
                throw new InvalidOperationException("Cấu hình EmailSettings:AppPassword chưa đầy đủ.");
            }

            if (!MailAddress.TryCreate(senderEmail, out var senderAddress))
            {
                throw new InvalidOperationException("Cấu hình EmailSettings:SenderEmail không hợp lệ.");
            }

            var recipientEmail = toEmail?.Trim();
            if (string.IsNullOrWhiteSpace(recipientEmail) || !MailAddress.TryCreate(recipientEmail, out var recipientAddress))
            {
                throw new InvalidOperationException("Email người nhận không hợp lệ.");
            }

            _logger.LogInformation(
                "Bat dau gui email. Operation={Operation}, Recipient={Recipient}, Sender={Sender}, SmtpHost={SmtpHost}, Port={Port}.",
                "SendAsync",
                MaskEmail(recipientEmail),
                MaskEmail(senderEmail),
                smtpServer,
                port);

            if (string.IsNullOrWhiteSpace(subject))
            {
                throw new InvalidOperationException("Tiêu đề email không được để trống.");
            }

            if (string.IsNullOrWhiteSpace(body))
            {
                throw new InvalidOperationException("Nội dung email không được để trống.");
            }

            using var mail = new MailMessage
            {
                From = new MailAddress(senderAddress.Address, senderName),
                Subject = subject,
                Body = body,
                IsBodyHtml = false,
                SubjectEncoding = Encoding.UTF8,
                BodyEncoding = Encoding.UTF8,
                HeadersEncoding = Encoding.UTF8
            };
            mail.To.Add(recipientAddress);

            using var smtp = new SmtpClient(smtpServer, port)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(username, appPassword)
            };

            try
            {
                await smtp.SendMailAsync(mail);
                _logger.LogInformation(
                    "SMTP da chap nhan email. Operation={Operation}, Recipient={Recipient}.",
                    "SendAsync",
                    MaskEmail(recipientEmail));
            }
            catch (SmtpException ex)
            {
                _logger.LogError(
                    ex,
                    "SMTP gui email that bai. Operation={Operation}, Recipient={Recipient}, StatusCode={StatusCode}, ExceptionType={ExceptionType}.",
                    "SendAsync",
                    MaskEmail(recipientEmail),
                    ex.StatusCode,
                    ex.GetType().Name);

                throw new InvalidOperationException(ToUserFriendlySmtpMessage(ex), ex);
            }
        }

        public Task SendAccountActivationEmailAsync(
            string toEmail,
            string recipientName,
            string userName,
            string activationUrl,
            int lifetimeHours)
        {
            if (string.IsNullOrWhiteSpace(activationUrl))
            {
                throw new InvalidOperationException("Liên kết kích hoạt tài khoản không hợp lệ.");
            }

            var body = $"""
Xin chào {recipientName},

Tài khoản của bạn trên hệ thống Quản lý dự án AI đã được tạo.

Tên đăng nhập: {userName}

Để đặt mật khẩu và kích hoạt tài khoản, vui lòng mở liên kết sau trong thời hạn {lifetimeHours} giờ:
{activationUrl}

Liên kết này chỉ sử dụng được một lần.

Nếu bạn không yêu cầu tài khoản này, vui lòng bỏ qua email.

Trân trọng,
Hệ thống Quản lý dự án AI
""";

            return SendAsync(toEmail, "Kích hoạt tài khoản Quản lý dự án AI", body);
        }

        private static string ToUserFriendlySmtpMessage(SmtpException ex)
        {
            if (ex.StatusCode == SmtpStatusCode.ClientNotPermitted
                || ex.StatusCode == SmtpStatusCode.MustIssueStartTlsFirst
                || ex.StatusCode == SmtpStatusCode.CommandNotImplemented
                || ex.StatusCode == SmtpStatusCode.GeneralFailure)
            {
                return "Không thể kết nối máy chủ SMTP Gmail. Vui lòng kiểm tra mạng hoặc thử lại sau.";
            }

            if (ex.StatusCode == SmtpStatusCode.ClientNotPermitted
                || ex.StatusCode == SmtpStatusCode.CommandUnrecognized
                || ex.StatusCode == SmtpStatusCode.CommandParameterNotImplemented)
            {
                return "Cấu hình SMTP Gmail chưa hợp lệ. Vui lòng kiểm tra lại cấu hình email.";
            }

            if (ex.StatusCode == SmtpStatusCode.MailboxNameNotAllowed
                || ex.StatusCode == SmtpStatusCode.MailboxUnavailable
                || ex.StatusCode == SmtpStatusCode.UserNotLocalTryAlternatePath)
            {
                return "Địa chỉ email người nhận không hợp lệ hoặc không khả dụng.";
            }

            if (ex.InnerException is TimeoutException)
            {
                return "Gửi email kích hoạt bị quá thời gian chờ. Vui lòng thử lại sau.";
            }

            if (ex.Message.Contains("authentication", StringComparison.OrdinalIgnoreCase)
                || ex.Message.Contains("username", StringComparison.OrdinalIgnoreCase)
                || ex.Message.Contains("password", StringComparison.OrdinalIgnoreCase))
            {
                return "Xác thực SMTP Gmail thất bại. Vui lòng kiểm tra Username/App Password.";
            }

            return "Không gửi được email kích hoạt. Vui lòng kiểm tra cấu hình Gmail hoặc thử lại sau.";
        }

        private static string MaskEmail(string? email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return "[empty]";
            }

            var trimmed = email.Trim();
            var atIndex = trimmed.IndexOf('@');
            if (atIndex <= 1)
            {
                return "***";
            }

            return $"{trimmed[0]}***{trimmed[(atIndex - 1)..]}";
        }
    }
}
