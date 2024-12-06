namespace DATN.Entities
{
    public class ConfirmEmail : BaseEntity
    {
        public int UserId { get; set; }
        public User? User { get; set; }
        public DateTime RequiredDateTime { get; set; }
        public DateTime ExpiredDateTime { get; set; }
        public string ConfirmCode { get; set; }
        public bool IsConfirm { get; set; }
     
    }
}
