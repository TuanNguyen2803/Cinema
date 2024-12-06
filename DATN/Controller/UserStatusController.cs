using DATN.Payload.Request.CinemaRequest;
using DATN.Payload.Request.UserStatusRequest;
using DATN.Service.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DATN.Controller
{
    [ApiController]
    public class UserStatusController : ControllerBase
    {
        private readonly IUserStatusService _userStatusService;
        public UserStatusController(IUserStatusService userStatusService)
        {
            _userStatusService = userStatusService;
        }
        [HttpPost("Thêm UserStatus")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Roles = "Admin")]
        public IActionResult Them(CreateUserStatus request)
        {
            return Ok(_userStatusService.CreateUserStatus(request));
        }
        [HttpPut("Sửa UserStatus")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Roles = "Admin")]
        public IActionResult Sua(UpdateUserStatus request)
        {
            return Ok(_userStatusService.UpdateUserStatus(request));
        }
        [HttpDelete("Xóa UserStatus")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Roles = "Admin")]
        public IActionResult Xoa(DeleteUserStatus request)
        {
            return Ok(_userStatusService.DeleteUserStatus(request));
        }
        [HttpGet("Hiển thị UserStatus")]
        public IActionResult HienThi(int pageSize, int pageNumber)
        {
            return Ok(_userStatusService.HienThi(pageSize, pageNumber));
        }

    }
}
