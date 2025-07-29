using Microsoft.AspNetCore.Mvc;
using ObsidianIQ.FormsAPI.Models;
using ObsidianIQ.FormsAPI.Services;

namespace ObsidianIQ.FormsAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WorkWithUsController : ControllerBase
    {
        private readonly IFormService _formService;
        private readonly ILogger<WorkWithUsController> _logger;

        public WorkWithUsController(IFormService formService, ILogger<WorkWithUsController> logger)
        {
            _formService = formService;
            _logger = logger;
        }

        /// <summary>
        /// Submit a work with us form
        /// </summary>
        /// <param name="model">The work with us form data</param>
        /// <returns>Form submission response</returns>
        [HttpPost]
        public async Task<IActionResult> SubmitForm([FromBody] WorkWithUsFormModel model)
        {
            try
            {
                _logger.LogInformation("Work With Us form submission received from IP: {IP}", 
                    HttpContext.Connection.RemoteIpAddress);

                // Debug logging to see what data is received
                _logger.LogInformation("Received Work With Us Form Data:");
                _logger.LogInformation("  FirstName: '{FirstName}'", model.FirstName ?? "NULL");
                _logger.LogInformation("  LastName: '{LastName}'", model.LastName ?? "NULL");
                _logger.LogInformation("  Email: '{Email}'", model.Email ?? "NULL");
                _logger.LogInformation("  PhoneNumber: '{PhoneNumber}'", model.PhoneNumber ?? "NULL");
                _logger.LogInformation("  Message: '{Message}'", model.Message ?? "NULL");
                _logger.LogInformation("  HasResume: '{HasResume}'", model.HasResume);

                var response = await _formService.ProcessWorkWithUsFormAsync(model);

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
                _logger.LogError(ex, "Error processing work with us form submission");
                return StatusCode(500, new FormSubmissionResponse
                {
                    Success = false,
                    Message = "An internal server error occurred"
                });
            }
        }

        /// <summary>
        /// Health check endpoint for work with us form service
        /// </summary>
        /// <returns>Service status</returns>
        [HttpGet("health")]
        public IActionResult HealthCheck()
        {
            return Ok(new { status = "healthy", service = "work-with-us-form" });
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
