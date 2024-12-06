
using DATN.Payload.Request.BannerRequest;
using DATN.Payload.Request.CinemaRequest;
using DATN.Service.Implement;
using DATN.Service.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DATN.Controller
{

    [ApiController]
    public class BannerController : ControllerBase
    {
        private readonly IBannerService _bannerService;
        public BannerController(IBannerService bannerService)
        {
            _bannerService = bannerService;
        }
        [HttpPost("Thêm banner")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //[Authorize(Roles = "Admin")]
        //[Consumes(contentType: "multipart/form-data")]
        public async Task<IActionResult> Them(ThemBannerRequest request)
        {
            return Ok(await _bannerService.ThemBanner(request));
        }
        [HttpPut("Sửa banner")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //[Authorize(Roles = "Admin")]
        //[Consumes(contentType: "multipart/form-data")]
        public async Task<IActionResult> Sua(SuaBannerRequest request)
        {
            return Ok(await _bannerService.SuaBanner(request));
        }
        [HttpDelete("Xóa banner")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //[Authorize(Roles = "Admin")]
        public IActionResult Xoa(int bannerid)
        {
            return Ok(_bannerService.XoaBanner(bannerid));
        }
        [HttpGet("Hiển thị")]
        public IActionResult GetAll()
        {
            return Ok(_bannerService.Hienthi());
        }
        [HttpGet("Hienthitheobannerid")]
        public IActionResult Hienthitheoid(int bannerid)
        {
            return Ok(_bannerService.Hienthitheoid(bannerid));
        }

    }
}
