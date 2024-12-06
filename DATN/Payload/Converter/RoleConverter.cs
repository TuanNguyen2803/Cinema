using DATN.Entities;
using DATN.Payload.DTO;

namespace DATN.Payload.Converter
{
    public class RoleConverter
    {
        public RoleDTO EntityToDTO(Role role)
        {
            return new RoleDTO
            {
                Id = role.Id,
                Code = role.Code,
                RoleName=role.RoleName,
            };
        }
    }
}
