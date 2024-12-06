using DATN.Payload.DTO;
using DATN.Payload.Request.RoleRequest;
using DATN.Payload.Response;

namespace DATN.Service.Interface
{
    public interface IRoleService
    {
        ResponseObject<RoleDTO> CreateRole(CreateRole request);
        ResponseObject<RoleDTO> UpdateRole(UpdateRole request);
        ResponseObject<RoleDTO> DeleteRole(DeleteRole request);
        IQueryable<RoleDTO> HienThi();
    }
}
