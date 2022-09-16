using System.Collections.Generic;

namespace AutoTelegram.Models
{
    public class AuthDto
    {
        public string? ApiId { get; set; }
        public string? ApiHash { get; set; }
        public string? PhoneNumber { get; set; }
        public List<Message>? Messages { get; set; }
    }
}
