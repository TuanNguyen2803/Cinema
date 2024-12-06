using DATN.Entities;
using DATN.Payload.DTO;

namespace DATN.Payload.Converter
{
    public class UserStatusConverter
    {
        public UserStatusDTO EntityToDTO(UserStatus userStatus)
        {
            return new UserStatusDTO
            {
                Id = userStatus.Id,
                Code = userStatus.Code,
                Name = userStatus.Name, 
            };
        }
    }
}
