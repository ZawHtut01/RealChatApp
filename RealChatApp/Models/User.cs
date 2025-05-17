using System.ComponentModel.DataAnnotations;

namespace RealChatApp.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        public string? ProfileImagePath { get; set; }

        public ICollection<ChatMessage> SentMessages { get; set; }
        public ICollection<ChatMessage> ReceivedMessages { get; set; }
    }
}
