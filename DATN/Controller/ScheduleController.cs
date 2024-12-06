using DATN.Payload.Request.CinemaRequest;
using DATN.Payload.Request.ScheduleRequest;
using DATN.Service.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DATN.Controller
{
    [ApiController]
    public class ScheduleController : ControllerBase
    {
        private readonly IScheduleService _scheduleService;
        public ScheduleController(IScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }
        [HttpPost("Thêm schedule")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //[Authorize(Roles = "Admin,Manager")]
        public IActionResult Them(ThemScheduleRequest request)
        {
            return Ok(_scheduleService.Them(request));
        }

        [HttpPost("Themlichchieutufile")]
        public IActionResult Themlichchieutufile(IFormFile excelFile)
        {
            return Ok(_scheduleService.ThemLichChieuTuFileExcel(excelFile));
        }
        [HttpPut("Sửa Schedule")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //[Authorize(Roles = "Admin,Manager")]
        public IActionResult Sua(SuaScheduleRequest request)
        {
            return Ok(_scheduleService.Sua(request));
        }
        [HttpDelete("Xóa Schedule")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //[Authorize(Roles = "Admin,Manager")]
        public IActionResult Xoa(int schdeuleid)
        {
            return Ok(_scheduleService.Xoa(schdeuleid));
        }
        [HttpGet("Hienthilichchieu")]
        public IActionResult Hienthi( int movieid, int? cinemaid)
        {
            return Ok(_scheduleService.Getall(movieid,cinemaid));
        }
        [HttpGet("Hienthilichchieutheoid")]
        public IActionResult Hienthitheoid(int scheduleid)
        {
            return Ok(_scheduleService.hienthitheoid(scheduleid));
        }
        [HttpGet("timkiemlichchieu")]
        public IActionResult Timkiemlichchieu(string keyword)
        {
            return Ok(_scheduleService.TimKiemLichChieu(keyword));
        }
        [HttpGet("hienthischedule")]
        public IActionResult hienthischedule(int? cinemaId, int? roomId, int? movieId)
        {
            return Ok(_scheduleService.GetSchedules(cinemaId, roomId, movieId));
        }
        [HttpGet("Hienthilichchieutheorap")]
        public IActionResult Hienthitheorap(int movieid,int? cinemaid)
        {
            return Ok(_scheduleService.GetbyRap(movieid, cinemaid));
        }
    }
}
