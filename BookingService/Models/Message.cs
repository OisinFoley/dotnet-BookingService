using System;
using System.ComponentModel.DataAnnotations;

namespace BookingService.Models
{
    public class Message
    {
        [Key]
        public Guid Id { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
    }
}
