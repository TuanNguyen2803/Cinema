using DATN.Payload.Request.BannerRequest;
using DATN.Payload.Request.GeneralSettingRequest;
using DATN.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DATN.Controller
{
    [ApiController]
    public class GeneralSettingController : ControllerBase
    {
        private readonly IGeneralSettingService _generalSettingService;
        public GeneralSettingController(IGeneralSettingService generalSettingService)
        {
            _generalSettingService = generalSettingService;
        }
        [HttpPost("Thêm generalSetting")]
        public IActionResult Them(ThemGeneralSettingRequest request)
        {
            return Ok(_generalSettingService.Them(request));
        }
        [HttpPut("Sửa generalSetting")]
        public IActionResult Sua(SuaGeneralSettingRequest request)
        {
            return Ok(_generalSettingService.Sua(request));
        }
        [HttpDelete("Xóa generalSetting")]
        public IActionResult Xoa(XoaGeneralSettingRequest request)
        {
            return Ok(_generalSettingService.Xoa(request));
        }
        [HttpGet("Hiển thị generalSetting")]
        public IActionResult Hienthi(int pageSize,int pageNumber)
        {
            return Ok(_generalSettingService.Hienthi(pageSize,pageNumber));
        }
    }
}
