using DATN.Payload.Request.CinemaRequest;
using DATN.Service.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DATN.Controller
{

    [ApiController]
    public class CinemaController : ControllerBase
    {
        private readonly ICinemaService _cinemaService;
        public CinemaController(ICinemaService cinemaService)
        {
            _cinemaService = cinemaService;
        }
        [HttpPost("Thêm cinema")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //[Authorize(Roles = "Admin")]
        public IActionResult Them(ThemCinemaRequest request)
        {
            return Ok(_cinemaService.Them(request));
        }
        [HttpPut("Sửa cinema")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //[Authorize(Roles = "Admin")]
        public IActionResult Sua(SuaCinemaRequest request)
        {
            return Ok(_cinemaService.Sua(request));
        }
        [HttpDelete("Xóa cinema")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //[Authorize(Roles = "Admin")]
        public IActionResult Xoa(int cinemaid)
        {
            return Ok(_cinemaService.Xoa(cinemaid));
        }
        [HttpGet("Hienthicinema")]
        public IActionResult Hienthi()
        {
            return Ok(_cinemaService.Getall());
        }
        [HttpGet("Hienthicinematheoid")]
        public IActionResult Hienthitheoid(int cinemaid)
        {
            return Ok(_cinemaService.laytheoid(cinemaid));
        }
        [HttpGet("Timkiemcinema")]
        public IActionResult Timkiemcinema(string keyword)
        {
            return Ok(_cinemaService.TimKiem(keyword));
        }

    }
}
