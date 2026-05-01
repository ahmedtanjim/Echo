using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Echo.Api.Models
{
    public class ChatMessage
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string SenderId { get; set; } = string.Empty;
        public string SenderName { get; set; } = string.Empty;

        public string ReceiverId { get; set; } = string.Empty;

        public string Text { get; set; } = string.Empty;
        public string? AttachmentUrl { get; set; }
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
    }
}
