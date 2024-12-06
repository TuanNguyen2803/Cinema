namespace DATN.Payload.DTO
{
    public class GetUserDTO
    {
        public int Id { get; set; }
        public int Point { get; set; } = 0;
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public int UserStatusId { get; set; }
        public int? RankCustomerId { get; set; }
        public bool? IsActive { get; set; }
        public int RoleId { get; set; }

    }
}
