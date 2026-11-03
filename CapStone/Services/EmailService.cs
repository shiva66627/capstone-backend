
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace CapStone.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendOtpEmailAsync(string email, string otp)
        {
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress(
                "CapStone",
                _configuration["EmailSettings:SenderEmail"]!));

            message.To.Add(MailboxAddress.Parse(email));

            message.Subject = "CapStone Email Verification OTP";

            message.Body = new TextPart("plain")
            {
                Text = $"Your CapStone verification OTP is {otp}. " +
                       "It expires in 5 minutes. Do not share it with anyone."
            };

            using var smtp = new SmtpClient();

            await smtp.ConnectAsync(
                _configuration["EmailSettings:SmtpServer"],
                int.Parse(_configuration["EmailSettings:Port"]!),
                SecureSocketOptions.StartTls);

            await smtp.AuthenticateAsync(
                _configuration["EmailSettings:SenderEmail"],
                _configuration["EmailSettings:AppPassword"]);

            await smtp.SendAsync(message);
            await smtp.DisconnectAsync(true);
        }
    }
}
