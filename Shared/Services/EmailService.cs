using Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Shared.Services
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;

        public EmailService(ILogger<EmailService> logger)
        {
            _logger = logger;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            _logger.LogWarning($"[EMAIL MOCK] Enviando a {to}: {subject}");
            _logger.LogWarning($"[EMAIL BODY] {body}");
            await Task.CompletedTask;
        }
    }
}
