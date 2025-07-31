using Microsoft.AspNetCore.Mvc;
using ObsidianIQ.FormsAPI.Models;
using ObsidianIQ.FormsAPI.Services;

namespace ObsidianIQ.FormsAPI.Controllers
{
    [ApiController]
    [Route("api/contact-us")]
    public class ContactController : ControllerBase
    {
        private readonly IFormService _formService;
        private readonly ILogger<ContactController> _logger;
        private readonly ObsidianIQ.FormsAPI.Models.EmailSettings _emailSettings;

        public ContactController(
            IFormService formService,
            ILogger<ContactController> logger,
            Microsoft.Extensions.Options.IOptions<ObsidianIQ.FormsAPI.Models.EmailSettings> emailSettings)
        {
            _formService = formService;
            _logger = logger;
            _emailSettings = emailSettings.Value;
        }

        /// <summary>
        /// Submit a contact form
        /// </summary>
        /// <param name="model">The contact form data</param>
        /// <returns>Form submission response</returns>
        [HttpPost]
        public async Task<IActionResult> SubmitForm([FromBody] ContactFormModel model)
        {
            try
            {
                _logger.LogInformation("Contact form submission received from IP: {IP}", 
                    HttpContext.Connection.RemoteIpAddress);

                // Debug logging to see what data is received
                _logger.LogInformation("Received Contact Form Data:");
                _logger.LogInformation("  ServiceRequested: '{ServiceRequested}'", model.ServiceRequested ?? "NULL");
                _logger.LogInformation("  FullName: '{FullName}'", model.FullName ?? "NULL");
                _logger.LogInformation("  Email: '{Email}'", model.Email ?? "NULL");
                _logger.LogInformation("  PhoneNumber: '{PhoneNumber}'", model.PhoneNumber ?? "NULL");
                _logger.LogInformation("  CompanyName: '{CompanyName}'", model.CompanyName ?? "NULL");
                _logger.LogInformation("  Position: '{Position}'", model.Position ?? "NULL");
                _logger.LogInformation("  Country: '{Country}'", model.Country ?? "NULL");
                _logger.LogInformation("  StateProvince: '{StateProvince}'", model.StateProvince ?? "NULL");
                _logger.LogInformation("  Message: '{Message}'", model.Message ?? "NULL");

                // Example usage of _emailSettings to build SMTP client
                // (Replace this with your actual email sending logic as needed)
                using var smtpClient = new System.Net.Mail.SmtpClient(_emailSettings.SmtpHost, _emailSettings.SmtpPort)
                {
                    EnableSsl = _emailSettings.EnableSsl,
                    Credentials = new System.Net.NetworkCredential(_emailSettings.SmtpUsername, _emailSettings.SmtpPassword)
                };
                // You can now use smtpClient to send emails as needed

                var response = await _formService.ProcessContactFormAsync(model);

                if (response.Success)
                {
                    return Ok(response);
                }
                else
                {
                    return BadRequest(response);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing contact form submission");
                return StatusCode(500, new FormSubmissionResponse
                {
                    Success = false,
                    Message = "An internal server error occurred"
                });
            }
        }

        /// <summary>
        /// Health check endpoint for contact form service
        /// </summary>
        /// <returns>Service status</returns>
        [HttpGet("health")]
        public IActionResult HealthCheck()
        {
            return Ok(new { status = "healthy", service = "contact-form" });
        }

        /// <summary>
        /// Handle preflight OPTIONS requests for CORS
        /// </summary>
        /// <returns>OK response for preflight</returns>
        [HttpOptions]
        public IActionResult HandleOptions()
        {
            return Ok();
        }
    }
}
