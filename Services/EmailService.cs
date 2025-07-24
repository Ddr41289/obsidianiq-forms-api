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
                // TODO: Implement actual email sending logic
                // This could use SendGrid, SMTP, or other email services
                
                _logger.LogInformation("Contact form submission received from {Email} at {Time}", 
                    contactForm.Email, contactForm.SubmittedAt);

                // For now, just log the form data
                _logger.LogInformation("Contact Form Data: {@ContactForm}", contactForm);

                // Simulate email sending delay
                await Task.Delay(100);

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
                // TODO: Implement actual email sending logic
                // This could use SendGrid, SMTP, or other email services
                
                _logger.LogInformation("Work With Us form submission received from {Email} at {Time}", 
                    workWithUsForm.Email, workWithUsForm.SubmittedAt);

                // For now, just log the form data
                _logger.LogInformation("Work With Us Form Data: {@WorkWithUsForm}", workWithUsForm);

                // Simulate email sending delay
                await Task.Delay(100);

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
