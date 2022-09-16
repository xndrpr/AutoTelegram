using System.Collections.Generic;

namespace AutoTelegram.Models
{
    public class AuthDto
    {
        public string? ApiId { get; set; }
        public string? ApiHash { get; set; }
        public string? PhoneNumber { get; set; }

        public AuthDto(string? apiId, string? apiHash, string? phoneNumber)
        {
            ApiId = apiId;
            ApiHash = apiHash;
            PhoneNumber = phoneNumber;
        }

        public string? Config(string what)
        {
            switch (what)
            {
                case "api_id":
                    return ApiId;
                case "api_hash":
                    return ApiHash;
                case "phone_number":
                    return PhoneNumber;
                case "verification_code":
                    var code = Microsoft.VisualBasic.Interaction.InputBox("Code: ", "Verification Code");
                    return code;
                default:
                    return null;
            }
        }
    }
}
