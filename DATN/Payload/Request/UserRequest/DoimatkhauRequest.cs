namespace DATN.Payload.Request.UserRequest
{
    public class DoimatkhauRequest
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmNewPassword { get; set; }
    }
}
