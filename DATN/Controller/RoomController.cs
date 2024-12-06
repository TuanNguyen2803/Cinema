using DATN.Payload.Request.CinemaRequest;
using DATN.Payload.Request.Room;
using DATN.Service.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DuAnPhim.Controller
{

    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly IRoomService _roomService;
        public RoomController(IRoomService roomService)
        {
            _roomService = roomService;
        }
        [HttpPost("Thêm room")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //[Authorize(Roles = "Admin")]
        public IActionResult Them(ThemRoomRequest request)
        {
            return Ok(_roomService.Them(request));
        }
        [HttpPost("Themroomtheofile")]
        public IActionResult Themroomtheofile(IFormFile excelfile)
        {
            return Ok(_roomService.ImportRoomFromExcel(excelfile));
        }
        [HttpPut("Sửa room")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //[Authorize(Roles = "Admin")]
        public IActionResult Sua(SuaRoomRequest request)
        {
            return Ok(_roomService.Sua(request));
        }
        [HttpDelete("Xóa room")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //[Authorize(Roles = "Admin")]
        public IActionResult Xoa(int roomid)
        {
            return Ok(_roomService.Xoa(roomid));
        }
        [HttpGet("hienthiroom")]

        public IActionResult Hienthiroom(int? cinemaid)
        {
            return Ok(_roomService.GetRooms(cinemaid));
        }
        [HttpGet("hienthiroomtheoid")]
        public IActionResult Hienthiroomtheoid(int roomid)
        {
            return Ok(_roomService.Hienthitheoid(roomid));
        }
        [HttpGet("hienthitatroom")]
        public IActionResult Hienthitat()
        {
            return Ok(_roomService.Hienthiroomtat());
        }
        [HttpGet("timkiemphong")]
        public IActionResult Timkiemphong(string keyword)
        {
            return Ok(_roomService.TimKiemRoom(keyword));
        }
    }
}
