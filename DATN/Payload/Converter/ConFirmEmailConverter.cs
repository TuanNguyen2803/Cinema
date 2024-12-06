using DATN.Entities;
using DATN.Payload.DTO;

namespace DATN.Payload.Converter
{
    public class ConFirmEmailConverter
    {
        public ConFirmEmailDTO EntityToDTO(ConfirmEmail confirmEmail)
        {
            return new ConFirmEmailDTO()
            {
                Id = confirmEmail.Id,
                ConfirmCode = confirmEmail.ConfirmCode,
                UserId = confirmEmail.UserId,
                RequiredDateTime = confirmEmail.RequiredDateTime,
                ExpiredDateTime = confirmEmail.ExpiredDateTime,
                IsConfirm= confirmEmail.IsConfirm,
            };
        }
    }
}
