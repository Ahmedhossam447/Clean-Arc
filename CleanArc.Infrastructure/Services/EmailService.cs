using CleanArc.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace CleanArc.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            await SendEmailAsync(to, subject, body, isHtml: false);
        }

        public async Task SendEmailAsync(string to, string subject, string body, bool isHtml)
        {
            var smtpServer = _configuration["EmailSettings:SmtpServer"];
            var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");
            var senderEmail = _configuration["EmailSettings:SenderEmail"];
            var senderPassword = _configuration["EmailSettings:SenderPassword"];

            using var client = new SmtpClient(smtpServer, smtpPort)
            {
                Credentials = new NetworkCredential(senderEmail, senderPassword),
                EnableSsl = true
            };

            var emailBody = isHtml ? WrapInHtmlTemplate(body) : body;

            var mailMessage = new MailMessage
            {
                From = new MailAddress(senderEmail!),
                Subject = subject,
                Body = emailBody,
                IsBodyHtml = isHtml
            };

            mailMessage.To.Add(to);

            await client.SendMailAsync(mailMessage);
        }

        private static string WrapInHtmlTemplate(string htmlMessage)
        {
            return $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <style>
                        body {{ font-family: 'Arial', sans-serif; line-height: 1.6; color: #333; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #eee; border-radius: 10px; background-color: #f9f9f9; }}
                        .header {{ text-align: center; padding-bottom: 20px; border-bottom: 2px solid #db98b7; margin-bottom: 20px; }}
                        .header h1 {{ color: #db98b7; margin: 0; font-size: 24px; }}
                        .content {{ background-color: #fff; padding: 20px; border-radius: 5px; }}
                        .footer {{ text-align: center; margin-top: 20px; font-size: 12px; color: #888; }}
                        .button {{ display: inline-block; padding: 10px 20px; background-color: #db98b7; color: #fff; text-decoration: none; border-radius: 5px; margin-top: 10px; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>HappyPaws Haven</h1>
                        </div>
                        <div class='content'>
                            {htmlMessage}
                        </div>
                        <div class='footer'>
                            <p>&copy; {DateTime.Now.Year} HappyPaws Haven. All rights reserved.</p>
                            <p>This is an automated message, please do not reply.</p>
                        </div>
                    </div>
                </body>
                </html>";
        }
    }
}
