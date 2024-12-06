using DATN.Payload.Request.Room;
using DATN.Payload.Request.SeatRequest;
using DATN.Service.Implement;
using DATN.Service.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DATN.Controller
{

    [ApiController]
    public class SeatController : ControllerBase
    {
        private readonly ISeatService _seatService;
        public SeatController(ISeatService seatService)
        {
            _seatService = seatService;
        }
        [HttpPost("Thêm seat")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //[Authorize(Roles = "Admin")]
        public IActionResult Them(ThemSeatRequest request)
        {
            return Ok(_seatService.Them(request));
        }
        [HttpPost("Themghetufile")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Roles = "Admin")]
        public IActionResult Themghetufile(IFormFile excel)
        {
            return Ok(_seatService.ThemGheTuFileExcel(excel));
        }
        [HttpPut("Sửa seat")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //[Authorize(Roles = "Admin")]
        public IActionResult Sua(SuaSeatRequest request)
        {
            return Ok(_seatService.Sua(request));
        }
        [HttpDelete("Xóa seat")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //[Authorize(Roles = "Admin")]
        public IActionResult Xoa(int seatid)
        {
            return Ok(_seatService.Xoa(seatid));
        }
        [HttpGet("Hienthiseat")]
     
        public IActionResult Hienthiseat()
        {
            return Ok(_seatService.Hienthi());
        }
        [HttpGet("Hienthiseattheophong")]
        public IActionResult GetSeatsByRoom(int roomId)
        {
      

            return Ok(_seatService.hienthitheophong(roomId));
        }
        [HttpGet("Hienthiseattheoid")]
        public IActionResult Hienthitheoid(int seatid)
        {


            return Ok(_seatService.hienthitheoid(seatid));
        }
    }
}
