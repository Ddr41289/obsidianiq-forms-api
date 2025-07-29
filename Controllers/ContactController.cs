using Microsoft.AspNetCore.Mvc;
using ObsidianIQ.FormsAPI.Models;
using ObsidianIQ.FormsAPI.Services;

namespace ObsidianIQ.FormsAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactController : ControllerBase
    {
        private readonly IFormService _formService;
        private readonly ILogger<ContactController> _logger;

        public ContactController(IFormService formService, ILogger<ContactController> logger)
        {
            _formService = formService;
            _logger = logger;
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
    }
}
