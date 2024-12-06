using DATN.Entities;

namespace DATN.Payload.DTO
{
    public class ConFirmEmailDTO
    {
        public int Id {  get; set; }
        public int UserId { get; set; }
        public DateTime RequiredDateTime { get; set; }
        public DateTime ExpiredDateTime { get; set; }
        public string ConfirmCode { get; set; }
        public bool IsConfirm { get; set; }
    }
}
