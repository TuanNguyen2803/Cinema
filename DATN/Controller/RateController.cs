using DATN.Payload.Request.RateRequest;
using DATN.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DATN.Controller
{
    [ApiController]
    public class RateController : ControllerBase
    {
        private readonly IRateService _rateService;
        public RateController(IRateService rateService)
        {
            _rateService = rateService;
        }
        [HttpPost("Thêm rate")]   
        
        public IActionResult Them(CreateRate request)
        {
            return Ok(_rateService.CreateRate(request));
        }
        [HttpPut("Sửa Rate")]
        public IActionResult Sua(UpdateRate request)
        {
            return Ok(_rateService.UpdateRate(request));
        }
        [HttpDelete("Xóa Rate")]
        public IActionResult Xoa(DeleteRate request)
        {
            return Ok(_rateService.DeleteRate(request));
        }
        [HttpGet("Hiển thị rate")]
        public IActionResult HienThi()
        {
            return Ok(_rateService.HienThi());
        }
    }
}
