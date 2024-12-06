using DATN.Payload.Request.BillStatusRequest;
using DATN.Service.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DATN.Controller
{

    [ApiController]
    public class BillStatusController : ControllerBase
    {
        private readonly IBillStatusService _billStatusService;
        public BillStatusController(IBillStatusService billStatusService)
        {
            _billStatusService = billStatusService;
        }
        [HttpPost("Thêm BillStatus")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Roles = "Admin")]
        public IActionResult Them(ThemBillStatusRequest request)
        {
            return Ok(_billStatusService.Them(request));
        }
        [HttpPut("Sửa BillStatus")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Roles = "Admin")]

        public IActionResult Sua(SuaBillStatusRequest request)
        {
            return Ok(_billStatusService.Sua(request));
        }
        [HttpDelete("Xóa BillStatus")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Roles = "Admin")]
        public IActionResult Xoa(XoaBillStatusRequest request)
        {
            return Ok(_billStatusService.Xoa(request));
        }
        [HttpGet("Hiển thị BillStatus")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Roles = "Admin")]
        public IActionResult Hienthi(int? Id, string? Name,int pageSize,int pageNumber)
        {
            return Ok(_billStatusService.HienThi(Id,Name,pageSize,pageNumber));
        }
    }
}
