using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cine_Critic_AI.Models
{
    public class ChatMessage
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Sender { get; set; } // "User" или "Bot"
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
    }
}

