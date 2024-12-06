using DATN.Payload.Request.SeatRequest;
using DATN.Payload.Request.SeatType;
using DATN.Service.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DATN.Controller
{

    [ApiController]
    public class SeatTypeController : ControllerBase
    {
        private readonly ISeatTypeService _seatTypeService;
        public SeatTypeController(ISeatTypeService seatTypeService)
        {
            _seatTypeService = seatTypeService;
        }
        [HttpPost("Thêm seatType")]
        public IActionResult Them(ThemSeatTypeRequest request)
        {
            return Ok(_seatTypeService.Create(request));
        }
        [HttpPut("Sửa seatType")]
        public IActionResult Sua(SuaSeatTypeRequest request)
        {
            return Ok(_seatTypeService.Update(request));
        }
        [HttpDelete("Xóa seatType")]
        public IActionResult Xoa(XoaSeatTypeRequest request)
        {
            return Ok(_seatTypeService.Delete(request));
        }
        [HttpGet("Hiển thị SeatType")]
        public IActionResult Hienthi(int pageSize, int pageNumber)
        {
            return Ok(_seatTypeService.HienThi(pageSize, pageNumber));
        }
    }
}

