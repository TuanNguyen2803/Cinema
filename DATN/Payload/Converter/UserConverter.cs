
using DATN.DataContext;
using DATN.Entities;
using DATN.Payload.Response;

namespace DATN.Payload.Converter
{
    public class UserConverter
    {
       
        public UserDTO EntitytoDTO(User user)
        {
            return new UserDTO
            {
                Id = user.Id,
                Point = user.Point,
                UserName = user.UserName,
                Email = user.Email,
                Name = user.Name,
                PhoneNumber = user.PhoneNumber,            
                RoleId = user.RoleId,
                RoleName=user.Role?.RoleName,
                UserstatusName=user.UserStatus?.Name,
                IsActive = user.IsActive,
                UserStatusId= user.UserStatusId,
                RankCustomerId=user.RankCustomerId,

            };
        }
    }
}


