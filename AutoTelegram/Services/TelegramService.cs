using AutoTelegram.Models;

namespace AutoTelegram.Services
{
    public class TelegramService
    {
        private readonly AuthDto _auth;
        public TelegramService(AuthDto auth)
        {
            _auth = auth;
        } 
    }
}
