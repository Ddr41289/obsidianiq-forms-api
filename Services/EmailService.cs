using ObsidianIQ.FormsAPI.Models;

namespace ObsidianIQ.FormsAPI.Services
{
    public interface IEmailService
    {
        Task<bool> SendContactFormEmailAsync(ContactFormModel contactForm);
        Task<bool> SendWorkWithUsFormEmailAsync(WorkWithUsFormModel workWithUsForm);
    }

    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly IConfiguration _configuration;

        public EmailService(ILogger<EmailService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<bool> SendContactFormEmailAsync(ContactFormModel contactForm)
        {
            try
            {
                _logger.LogInformation("Contact form submission received from {Email} at {Time}", contactForm.Email, contactForm.SubmittedAt);
                _logger.LogInformation("Contact Form Data: {@ContactForm}", contactForm);

                var smtpServer = _configuration["EmailSettings:SmtpServer"];
                var smtpPort = int.TryParse(_configuration["EmailSettings:SmtpPort"], out var port) ? port : 465;
                var smtpUsername = _configuration["EmailSettings:SmtpUsername"];
                var smtpPassword = _configuration["EmailSettings:SmtpPassword"];
                var fromEmail = _configuration["EmailSettings:FromEmail"];
                var toEmail = _configuration["EmailSettings:ToEmail"];

                if (string.IsNullOrWhiteSpace(fromEmail) || string.IsNullOrWhiteSpace(toEmail))
                {
                    _logger.LogError("FromEmail or ToEmail is not configured. Email not sent.");
                    return false;
                }

                using var client = new System.Net.Mail.SmtpClient(smtpServer, smtpPort)
                {
                    EnableSsl = true,
                    Credentials = new System.Net.NetworkCredential(smtpUsername, smtpPassword)
                };

                var mail = new System.Net.Mail.MailMessage
                {
                    From = new System.Net.Mail.MailAddress(fromEmail),
                    Subject = $"Contact Form Submission from {contactForm.FullName ?? contactForm.Email}",
                    Body = $"Service Requested: {contactForm.ServiceRequested}\nFull Name: {contactForm.FullName}\nEmail: {contactForm.Email}\nPhone: {contactForm.PhoneNumber}\nCompany: {contactForm.CompanyName}\nPosition: {contactForm.Position}\nCountry: {contactForm.Country}\nState/Province: {contactForm.StateProvince}\nMessage: {contactForm.Message}",
                    IsBodyHtml = false
                };
                mail.To.Add(toEmail);
                if (!string.IsNullOrWhiteSpace(contactForm.Email))
                {
                    mail.ReplyToList.Add(new System.Net.Mail.MailAddress(contactForm.Email));
                }

                await client.SendMailAsync(mail);
                _logger.LogInformation("Contact form email sent successfully to {ToEmail}", toEmail);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send contact form email for {Email}", contactForm.Email);
                return false;
            }
        }

        public async Task<bool> SendWorkWithUsFormEmailAsync(WorkWithUsFormModel workWithUsForm)
        {
            try
            {
                _logger.LogInformation("Work With Us form submission received from {Email} at {Time}", workWithUsForm.Email, workWithUsForm.SubmittedAt);
                _logger.LogInformation("Work With Us Form Data: {@WorkWithUsForm}", workWithUsForm);

                var smtpServer = _configuration["EmailSettings:SmtpServer"];
                var smtpPort = int.TryParse(_configuration["EmailSettings:SmtpPort"], out var port) ? port : 465;
                var smtpUsername = _configuration["EmailSettings:SmtpUsername"];
                var smtpPassword = _configuration["EmailSettings:SmtpPassword"];
                var fromEmail = _configuration["EmailSettings:FromEmail"];
                var toEmail = _configuration["EmailSettings:ToEmail"];

                if (string.IsNullOrWhiteSpace(fromEmail) || string.IsNullOrWhiteSpace(toEmail))
                {
                    _logger.LogError("FromEmail or ToEmail is not configured. Email not sent.");
                    return false;
                }

                using var client = new System.Net.Mail.SmtpClient(smtpServer, smtpPort)
                {
                    EnableSsl = true,
                    Credentials = new System.Net.NetworkCredential(smtpUsername, smtpPassword)
                };

                var fullName = $"{workWithUsForm.FirstName} {workWithUsForm.LastName}".Trim();
                var mail = new System.Net.Mail.MailMessage
                {
                    From = new System.Net.Mail.MailAddress(fromEmail),
                    Subject = $"Work With Us Form Submission from {fullName ?? workWithUsForm.Email}",
                    Body = $"Full Name: {fullName}\nEmail: {workWithUsForm.Email}\nPhone: {workWithUsForm.PhoneNumber}\nHas Resume: {workWithUsForm.HasResume}\nMessage: {workWithUsForm.Message}",
                    IsBodyHtml = false
                };
                mail.To.Add(toEmail);
                if (!string.IsNullOrWhiteSpace(workWithUsForm.Email))
                {
                    mail.ReplyToList.Add(new System.Net.Mail.MailAddress(workWithUsForm.Email));
                }

                await client.SendMailAsync(mail);
                _logger.LogInformation("Work With Us form email sent successfully to {ToEmail}", toEmail);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send work with us form email for {Email}", workWithUsForm.Email);
                return false;
            }
        }
    }
}
