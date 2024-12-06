
using DATN.Payload.DTO;
using DATN.Payload.Request.UserStatusRequest;
using DATN.Payload.Response;

namespace DATN.Service.Interface
{
    public interface IUserStatusService
    {
        ResponseObject<UserStatusDTO> CreateUserStatus(CreateUserStatus request);
        ResponseObject<UserStatusDTO> UpdateUserStatus(UpdateUserStatus request);
        ResponseObject<UserStatusDTO> DeleteUserStatus(DeleteUserStatus request);
        IQueryable<UserStatusDTO> HienThi(int pageSize, int pageNumber);
    }
}
