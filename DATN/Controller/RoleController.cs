using DATN.Payload.Request.RoleRequest;
using DATN.Service.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DATN.Controller
{

    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;
        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }
        [HttpPost("Thêm role")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Roles = "Admin")]
        public IActionResult Them(CreateRole request)
        {
            return Ok(_roleService.CreateRole(request));    
        }
        [HttpPut("Sửa Role")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Roles = "Admin")]
        public IActionResult Sua(UpdateRole request)
        {
            return Ok(_roleService.UpdateRole(request));
        }
        [HttpDelete("Xóa Role")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Roles = "Admin")]
        public IActionResult Xoa(DeleteRole request)
        {
            return Ok(_roleService.DeleteRole(request));
        }
        [HttpGet("hienthirole")]
        public IActionResult HienThi()
        {
            return Ok(_roleService.HienThi());
        }
    }
}
