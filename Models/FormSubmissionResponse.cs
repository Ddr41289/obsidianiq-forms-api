namespace ObsidianIQ.FormsAPI.Models
{
    public class FormSubmissionResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<string> Errors { get; set; } = new List<string>();
    }
}
