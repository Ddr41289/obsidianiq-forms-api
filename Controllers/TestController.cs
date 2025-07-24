using Microsoft.AspNetCore.Mvc;

namespace ObsidianIQ.FormsAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        /// <summary>
        /// Test endpoint to verify CORS configuration
        /// </summary>
        /// <returns>Test response with CORS headers</returns>
        [HttpGet]
        public IActionResult TestCors()
        {
            return Ok(new { 
                message = "CORS test successful", 
                timestamp = DateTime.UtcNow,
                origin = Request.Headers["Origin"].ToString(),
                server = "http://localhost:5000"
            });
        }

        /// <summary>
        /// Test POST endpoint to verify CORS configuration for form submissions
        /// </summary>
        /// <param name="testData">Test data</param>
        /// <returns>Test response</returns>
        [HttpPost]
        public IActionResult TestCorsPost([FromBody] object testData)
        {
            return Ok(new { 
                message = "CORS POST test successful", 
                timestamp = DateTime.UtcNow,
                receivedData = testData,
                origin = Request.Headers["Origin"].ToString(),
                server = "http://localhost:5000"
            });
        }

        /// <summary>
        /// Preflight request handler (OPTIONS)
        /// </summary>
        /// <returns>OK response for preflight</returns>
        [HttpOptions]
        public IActionResult PreflightCors()
        {
            return Ok();
        }
    }
}
