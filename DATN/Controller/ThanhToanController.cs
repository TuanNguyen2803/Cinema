using DATN.Payload.Request.BillRequest;
using DATN.Service.Implement;
using DATN.Service.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DATN.Controller
{
    
    [ApiController]
    public class ThanhToanController : ControllerBase
    {
        private readonly IVNPayService _vnpayService;
   
        public ThanhToanController(IVNPayService vnpayService)
        {
            _vnpayService = vnpayService;
           
        }
        [HttpPost("TaoDuongDanThanhToan")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public  IActionResult TaoDuongDanThanhToan( int hoaDonId)
        {
            int id = int.Parse(HttpContext.User.FindFirst("Id").Value);
            return Ok(_vnpayService.TaoDuongDanThanhToan(hoaDonId, HttpContext, id));
        }
        [HttpGet("Return")]
        public IActionResult Return()
        {
            var vnpayData = HttpContext.Request.Query;

            return Ok(_vnpayService.VNPayReturn(vnpayData));
        }
    }
}
