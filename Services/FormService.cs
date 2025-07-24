using FluentValidation;
using ObsidianIQ.FormsAPI.Models;

namespace ObsidianIQ.FormsAPI.Services
{
    public interface IFormService
    {
        Task<FormSubmissionResponse> ProcessContactFormAsync(ContactFormModel contactForm);
        Task<FormSubmissionResponse> ProcessWorkWithUsFormAsync(WorkWithUsFormModel workWithUsForm);
    }

    public class FormService : IFormService
    {
        private readonly IEmailService _emailService;
        private readonly IValidator<ContactFormModel> _contactFormValidator;
        private readonly IValidator<WorkWithUsFormModel> _workWithUsFormValidator;
        private readonly ILogger<FormService> _logger;

        public FormService(
            IEmailService emailService,
            IValidator<ContactFormModel> contactFormValidator,
            IValidator<WorkWithUsFormModel> workWithUsFormValidator,
            ILogger<FormService> logger)
        {
            _emailService = emailService;
            _contactFormValidator = contactFormValidator;
            _workWithUsFormValidator = workWithUsFormValidator;
            _logger = logger;
        }

        public async Task<FormSubmissionResponse> ProcessContactFormAsync(ContactFormModel contactForm)
        {
            var response = new FormSubmissionResponse();

            try
            {
                // Validate the form
                var validationResult = await _contactFormValidator.ValidateAsync(contactForm);
                if (!validationResult.IsValid)
                {
                    response.Success = false;
                    response.Message = "Validation failed";
                    response.Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    return response;
                }

                // Process the form (send email, save to database, etc.)
                var emailSent = await _emailService.SendContactFormEmailAsync(contactForm);
                
                if (emailSent)
                {
                    response.Success = true;
                    response.Message = "Thank you for contacting us! We'll get back to you soon.";
                    _logger.LogInformation("Contact form processed successfully for {Email}", contactForm.Email);
                }
                else
                {
                    response.Success = false;
                    response.Message = "There was an error processing your request. Please try again later.";
                    _logger.LogError("Failed to process contact form for {Email}", contactForm.Email);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error processing contact form for {Email}", contactForm.Email);
                response.Success = false;
                response.Message = "An unexpected error occurred. Please try again later.";
            }

            return response;
        }

        public async Task<FormSubmissionResponse> ProcessWorkWithUsFormAsync(WorkWithUsFormModel workWithUsForm)
        {
            var response = new FormSubmissionResponse();

            try
            {
                // Validate the form
                var validationResult = await _workWithUsFormValidator.ValidateAsync(workWithUsForm);
                if (!validationResult.IsValid)
                {
                    response.Success = false;
                    response.Message = "Validation failed";
                    response.Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    return response;
                }

                // Process the form (send email, save to database, etc.)
                var emailSent = await _emailService.SendWorkWithUsFormEmailAsync(workWithUsForm);
                
                if (emailSent)
                {
                    response.Success = true;
                    response.Message = "Thank you for your interest in working with us! We'll review your project details and get back to you soon.";
                    _logger.LogInformation("Work With Us form processed successfully for {Email}", workWithUsForm.Email);
                }
                else
                {
                    response.Success = false;
                    response.Message = "There was an error processing your request. Please try again later.";
                    _logger.LogError("Failed to process Work With Us form for {Email}", workWithUsForm.Email);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error processing Work With Us form for {Email}", workWithUsForm.Email);
                response.Success = false;
                response.Message = "An unexpected error occurred. Please try again later.";
            }

            return response;
        }
    }
}
