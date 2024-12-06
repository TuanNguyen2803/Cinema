using DATN.Payload.Request.BillFoodRequest;
using DATN.Payload.Request.BillRequest;
using DATN.Service.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

namespace DATN.Controller
{

    [ApiController]
    public class BillController : ControllerBase
    {
        private readonly IBillService _billService;
        public BillController(IBillService billService)
        {
            _billService = billService;
        }
        [HttpPost("Thembil")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult Them([FromBody] CreateBillRequest request)
        {
            int id = int.Parse(HttpContext.User.FindFirst("Id").Value);
            return Ok(_billService.Create(id, request));
        }
        [HttpGet("Doanhthutheorap")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

        public IActionResult Thongkedoanhso()
        {
            return Ok(_billService.ThongKeDoanhSo());
        }
        [HttpGet("Doanbanchay")]
        public IActionResult Thongke()
        {
                return Ok(_billService.Thongkedoan());
        }
        [HttpGet("Thongkedoanhthutheophim")]
        public IActionResult Thongketheophim()
        {
            return Ok(_billService.ThongKeDoanhThuTheoPhim());
        }
        [HttpGet("Doanhthuraptheongay")]
        public IActionResult Theongay(DateTime ngayThongKe)
        {
            return Ok(_billService.ThongKeTheoNgay(ngayThongKe));
        }
        [HttpGet("Doanhthuraptheothang")]
        public IActionResult Theothang(int month, int year)
        {
            return Ok(_billService.ThongKeTheoThang(month, year));
        }
        [HttpGet("Doanhthuraptheonam")]
        public IActionResult Theonam(int year)
        {
            return Ok(_billService.ThongKeTheoNam(year));
        }
        [HttpGet("Thongketheotungthang")]
        public IActionResult Thongketheotungthang()
        {
            return Ok(_billService.ThongKeDoanhThuTheoThang());
        }
        [HttpGet("Thongkedoanhthuthangtoanrap")]
        public IActionResult Daonhthutheothang()
        {
            return Ok(_billService.DoanhThuTheoThang());
        }
        [HttpGet("hoadontheouser")]
        public IActionResult Layhoadontheouser(int userid)
        {
            return Ok(_billService.GetBillsByUserId(userid));
        }
        [HttpGet("hoadontrongngay")]
        public IActionResult hoadontrongngay()
        {
            return Ok(_billService.GetBillsByDay());
        }
    }
}

