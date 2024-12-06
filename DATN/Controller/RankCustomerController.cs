using DATN.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DATN.Controller
{
   
    [ApiController]
    public class RankCustomerController : ControllerBase
    {
        private readonly IRankCustomerService _rankcustomer;
        public RankCustomerController(IRankCustomerService rankcustomer)
        {
            _rankcustomer = rankcustomer;
        }
        [HttpGet("Hienthirank")]
        public IActionResult Hienthitrank()
        {
            return Ok(_rankcustomer.HienThi());
        }
    }
}
