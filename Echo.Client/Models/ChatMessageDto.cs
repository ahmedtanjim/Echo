namespace Echo.Client.Models
{
    public class ChatMessageDto
    {
        public string User { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? AttachmentUrl { get; set; }
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
    }
}
