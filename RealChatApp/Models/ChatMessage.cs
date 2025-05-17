using System.ComponentModel.DataAnnotations;

namespace RealChatApp.Models
{
    public class ChatMessage
    {
        public int Id { get; set; }

        public int SenderId { get; set; }
        public User Sender { get; set; }

        public int ReceiverId { get; set; }
        public User Receiver { get; set; }

        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
    }


}
