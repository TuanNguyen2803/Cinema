using DATN.Entities;
using DATN.Payload.DTO;

namespace DATN.Payload.Converter
{
    public class GetUserConverter
    {
        public GetUserDTO EntitytoDTO(User user)
        {
            return new GetUserDTO
            {
                Id = user.Id,
                Point = user.Point,
                UserName = user.UserName,
                Email = user.Email,
                Name = user.Name,
                PhoneNumber = user.PhoneNumber,
                RoleId = user.RoleId,
                IsActive = user.IsActive,
                UserStatusId = user.UserStatusId,
                RankCustomerId = user.RankCustomerId,
            };
        }
    }
}
