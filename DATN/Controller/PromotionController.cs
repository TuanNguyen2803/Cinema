using DATN.Payload.Request.BannerRequest;
using DATN.Payload.Request.PromotionRequest;
using DATN.Service.Implement;
using DATN.Service.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DATN.Controller
{

    public class PromotionController : ControllerBase
    {
        private readonly IPromotionService _promotionService;
        public PromotionController(IPromotionService promotionService)
        {
            _promotionService = promotionService;
        }
        [HttpPost("Thempromotion")]
        public IActionResult Them([FromBody] CreatePromotion request)
        {
            return Ok(_promotionService.Them(request));
        }
        [HttpPut("Suapromotion")]
        public IActionResult Sua([FromBody] UpdatePromtion request)
        {
            return Ok(_promotionService.Sua(request));
        }
        [HttpDelete("Xoapromtion")]
        public IActionResult Xoa(int promotionid)
        {
            return Ok(_promotionService.Delete(promotionid));
        }
        [HttpGet("Hienthipromotion")]
        public IActionResult Hienthi()
        {
            return Ok(_promotionService.GetAll());
        }
        [HttpGet("Hienthipromotiontheoid")]
        public IActionResult Hienthitheoid(int promotionid)
        {
            return Ok(_promotionService.hienthitheoid(promotionid));
        }
    }
}
