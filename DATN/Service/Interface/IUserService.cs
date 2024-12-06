
using Azure;


using DATN.Payload.DTO;
using DATN.Payload.Request.TokenRequest;
using DATN.Payload.Request.UserRequest;
using DATN.Payload.Response;

namespace DATN.Service.Interface
{
    public interface IUserService
    {
        ResponseObject<UserDTO> Dangkytaikhoan(DangkyRequest request);
        ResponseObject<UserDTO> Xacthuctaikhoan(XacthucRequest request);
        ResponseObject<TokenDTO> Login(LoginRequest request);
        ResponseObject<UserDTO> Quenmatkhau(QuenmatkhauRequest request);
        ResponseObject<UserDTO> Taomatkhau(TaomatkhaumoiRequest request);
        ResponseObject<UserDTO> Doimatkhau(DoimatkhauRequest request, int userId);
        ResponseObject<TokenDTO> RenewAccessToken(RequestToken request);
        ResponseObject<UserDTO> DeleteUser(int userid);
        ResponseObject<UserDTO> Thaydoiquyenhan(PhanQuyenRequest request);
        IQueryable<UserDTO> GetAllUsers();
        ResponseObject<UserDTO> UpdateUser(UpdateUserRequest request);
        IQueryable<GetUserDTO> TimKiemNguoiDung(string keyword);

    }
}
