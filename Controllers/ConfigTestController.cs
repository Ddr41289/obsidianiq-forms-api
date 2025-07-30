using Microsoft.AspNetCore.Mvc;

namespace ObsidianIQ.FormsAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConfigTestController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ConfigTestController> _logger;

        public ConfigTestController(IConfiguration configuration, ILogger<ConfigTestController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        [HttpGet("email-config")]
        public IActionResult GetEmailConfig()
        {
            try
            {
                var emailSettings = new
                {
                    SmtpServer = _configuration["EmailSettings:SmtpServer"],
                    SmtpPort = _configuration["EmailSettings:SmtpPort"],
                    FromEmail = _configuration["EmailSettings:FromEmail"],
                    ToEmail = _configuration["EmailSettings:ToEmail"],
                    HasSmtpUsername = !string.IsNullOrEmpty(_configuration["EmailSettings:SmtpUsername"]),
                    HasSmtpPassword = !string.IsNullOrEmpty(_configuration["EmailSettings:SmtpPassword"]),
                    // Don't expose actual credentials for security
                    SmtpUsernameLength = _configuration["EmailSettings:SmtpUsername"]?.Length ?? 0,
                    SmtpPasswordLength = _configuration["EmailSettings:SmtpPassword"]?.Length ?? 0
                };

                _logger.LogInformation("Email configuration check: {@EmailSettings}", emailSettings);

                return Ok(emailSettings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve email configuration");
                return StatusCode(500, "Failed to retrieve configuration");
            }
        }

        [HttpGet("environment")]
        public IActionResult GetEnvironmentInfo()
        {
            var environmentInfo = new
            {
                EnvironmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
                MachineName = Environment.MachineName,
                ProcessorCount = Environment.ProcessorCount,
                WorkingSet = Environment.WorkingSet,
                // Check if specific environment variables exist (without exposing values)
                HasSmtpUsername = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("EmailSettings__SmtpUsername")),
                HasSmtpPassword = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("EmailSettings__SmtpPassword"))
            };

            _logger.LogInformation("Environment check: {@EnvironmentInfo}", environmentInfo);

            return Ok(environmentInfo);
        }
    }
}
