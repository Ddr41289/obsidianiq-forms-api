namespace ObsidianIQ.FormsAPI.Models
{
    public class WorkWithUsFormModel
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool HasResume { get; set; } = false;
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    }
}
