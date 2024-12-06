using DATN.Payload.Response;

namespace DATN.Payload.DTO
{
    public class TokenDTO
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public UserDTO DataResponseUser { get; set; }
    }
}
