using DATN.DataContext;
using DATN.Entities;
using DATN.Payload.Request.CinemaRequest;
using DATN.Payload.Request.FoodRequest;
using DATN.Service.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DATN.Controller
{

    [ApiController]
    public class FoodController : ControllerBase
    {
        private readonly IFoodService _foodService;
        private readonly AppDbContext _context;
        public FoodController(IFoodService foodService)
        {
            _foodService = foodService;
        }
        [HttpPost("Thêm food")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //[Authorize(Roles = "Admin")]
        //[Consumes(contentType: "multipart/form-data")]
        public async Task<IActionResult> Them([FromForm] ThemFoodRequest request)
        {
            return Ok(await _foodService.Them(request));
        }
        [HttpPut("Sửa food")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //[Authorize(Roles = "Admin")]
        //[Consumes(contentType: "multipart/form-data")]
        public async Task<IActionResult> Sua(SuaFoodRequest request)
        {
            return Ok(await _foodService.Sua(request));
        }
        [HttpDelete("Xóa food")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //[Authorize(Roles = "Admin")]
        public IActionResult Xoa(int foodid)
        {
            return Ok(_foodService.Xoa(foodid));
        }

        //[HttpGet("Thống kê đồ ăn bán chạy")]
        //public IActionResult Thongke(int pageSize,int pageNumber)
        //{
        //    return Ok(_foodService.Thongke(pageSize,pageNumber));
        //}
        [HttpGet("Hienthifood")]
        public IActionResult Hienthi()
        {
            return Ok(_foodService.Hienthi());
        }
        [HttpGet("Hienthifoodtheoid")]
        public IActionResult Hienthitheoid(int foodid)
        {
            return Ok(_foodService.Hienthitheoid(foodid));
        }
    }
}
