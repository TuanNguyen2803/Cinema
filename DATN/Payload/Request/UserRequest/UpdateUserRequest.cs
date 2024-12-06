namespace DATN.Payload.Request.UserRequest
{
    public class UpdateUserRequest
    {
        public int Userid {  get; set; }
        public int RankCustomerId { get; set; }
        public int UserStatusId { get; set; }
        public bool IsActive { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
