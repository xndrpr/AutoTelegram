using System.Collections.Generic;

namespace AutoTelegram.Models
{
    public class Message
    {
        public string Text { get; set; }
        public string? Targets { get; set; }
        public string? Answer { get; set; }
        public string Username { get; set; }
        public int SleepSeconds { get; set; }
    }
}
