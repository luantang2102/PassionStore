using PassionStore.Infrastructure.Settings;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace PassionStore.Infrastructure.Externals
{
    public class EmailService
    {
        private readonly EmailOption _emailOption;

        public EmailService(EmailOption emailOption)
        {
            _emailOption = emailOption ?? throw new ArgumentNullException(nameof(emailOption));
        }

        public async Task SendEmailAsync(string toEmail, string subject, string? plainText = null, string? htmlContent = null)
        {
            if (string.IsNullOrEmpty(_emailOption.ApiKey) || string.IsNullOrEmpty(_emailOption.FromEmail))
            {
                throw new Exception("SendGrid configuration is missing.");
            }

            var client = new SendGridClient(_emailOption.ApiKey);
            var from = new EmailAddress(_emailOption.FromEmail, _emailOption.FromName);
            var toAddress = new EmailAddress(toEmail);
            var msg = MailHelper.CreateSingleEmail(from, toAddress, subject, plainText ?? string.Empty, htmlContent ?? string.Empty);
            var response = await client.SendEmailAsync(msg);

            if (response.StatusCode != System.Net.HttpStatusCode.OK &&
                response.StatusCode != System.Net.HttpStatusCode.Accepted)
            {
                var errorBody = await response.Body.ReadAsStringAsync();
                throw new Exception($"Failed to send email: {errorBody}");
            }
        }
    }
}
