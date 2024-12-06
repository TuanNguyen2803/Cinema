namespace DATN.Entities
{
    public class User : BaseEntity
    {
        public int Point { get; set; } = 0;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int? RankCustomerId { get; set; } 
        public int UserStatusId { get; set; }
        public bool IsActive { get; set; } = false;
        public int RoleId { get; set; }
        public Role? Role { get; set; }
        public RankCustomer? RankCustomer { get; set; }
        public UserStatus? UserStatus { get; set; }
        public IEnumerable<Bill>? Bills { get; set; }
        public IEnumerable<RefreshToken>? RefreshTokens { get; set; }
        public IEnumerable<ConfirmEmail>? ConfirmEmails { get; set; }
    }
}
