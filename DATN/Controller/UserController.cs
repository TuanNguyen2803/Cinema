
using DATN.Payload.Request.UserRequest;
using DATN.Service.Implement;
using DATN.Service.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DATN.Controller
{

    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
   
      
        public UserController(IUserService userService)
        {
            _userService = userService;
       
        }
        [HttpPost("Dangkytaikhoan")]
        public IActionResult ThemTaiKhoan(DangkyRequest request)
        {
            return Ok(_userService.Dangkytaikhoan(request));

        }
        [HttpPost("Xacthuc")]
        public IActionResult Xacthuctaikhoan(XacthucRequest request)
        {
            return Ok(_userService.Xacthuctaikhoan(request));
        }
        [HttpPut("Taomatkhaumoi")]
        public IActionResult Taomatkhaumoi(TaomatkhaumoiRequest request)
        {
            return Ok(_userService.Taomatkhau(request));
        }
        [HttpPut("Quenmatkhau")]
        public IActionResult Quenmatkhau(QuenmatkhauRequest request)
        {
            return Ok(_userService.Quenmatkhau(request));
        }
        [HttpPut("Changepass")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult DoiMatkhau(DoimatkhauRequest request)
        {
            int id = int.Parse(HttpContext.User.FindFirst("Id").Value);
            return Ok(_userService.Doimatkhau(request, id));
        }

        [HttpPost("Login")]
        public IActionResult Login(LoginRequest request)
        {
            var result = _userService.Login(request);
            if (result != null)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(new { message = "Thông tin đăng nhập không chính xác." });
            }
        }

        [HttpPut("Thaydoiquyen")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //[Authorize(Roles = "Admin")]

        public IActionResult Thaydoiquyen([FromForm] PhanQuyenRequest request)
        {
            
            return Ok(_userService.Thaydoiquyenhan(request));
        }
        [HttpDelete("Xoanguoidung")]
        public IActionResult Xoa(int userid)
        {
            return Ok(_userService.DeleteUser(userid));
        }
        [HttpGet("Hienthiuser")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //[Authorize(Roles = "Admin")]
        public IActionResult Hienthi()
        {
            return Ok(_userService.GetAllUsers());
        }
        [HttpPut("Suauser")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //[Authorize(Roles = "Admin")]
        public IActionResult Update(UpdateUserRequest request)
        {
            return Ok(_userService.UpdateUser(request));
        }
        [HttpGet("timkiemuser")]
        public IActionResult TimiemUser(string keyword)
        {
            return Ok(_userService.TimKiemNguoiDung(keyword));
        }
    }
}
