
namespace ObsidianIQ.FormsAPI.Models
{
    public class ContactFormModel
    {
        public string ServiceRequested { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? CompanyName { get; set; }
        public string? Position { get; set; }
        public string Country { get; set; } = string.Empty;
        public string StateProvince { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    }
}