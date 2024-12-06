using DATN.Entities;
using DATN.Handle.InputMovie;
using DATN.Payload.Request.FoodRequest;
using DATN.Payload.Request.MovieRequest;
using DATN.Service.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DATN.Controller
{

    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly IMovieService _movieService;
        public MovieController(IMovieService movieService)
        {
            _movieService = movieService;
        }
        [HttpPost("Thêm movie")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //[Authorize(Roles = "Admin")]
        //[Consumes(contentType: "multipart/form-data")]
        public async Task<IActionResult> Them([FromForm] ThemMovieRequest request)
        {
            return Ok(await _movieService.Them(request));
        }
        [HttpPost("themphimtufileexcel")]
        public async Task<IActionResult> ImportMoviesFromExcel(IFormFile excelFile)
        {
            return Ok(await _movieService.ImportMoviesFromExcel(excelFile));
          
        }

        [HttpPut("Sửa movie")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //[Authorize(Roles = "Admin")]
        //[Consumes(contentType: "multipart/form-data")]
        public async Task<IActionResult> Sua([FromForm] SuaMovieRequest request) // Đảm bảo dùng [FromForm]
        {
            return Ok(await _movieService.Sua(request));
        }
        [HttpDelete("Xóa movie")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //[Authorize(Roles = "Admin")]
        public IActionResult Xoa(int movieid)
        {
            return Ok(_movieService.Xoa(movieid));
        }

        [HttpGet("Hiển thị tất cả phim")]
        public IActionResult HienthiTatCa()
        {
            return Ok(_movieService.HienThiTatCaPhim());
        }
        [HttpGet("Hienthithongtin")]
        public IActionResult Hienthithongtinphim(int moveid)
        {
            return Ok(_movieService.Hienthithongtinphim(moveid));
        }
        [HttpGet("timkiemphim")]
        public IActionResult Timkiemphim(string keyword)
        {
            return Ok(_movieService.TimKiemPhim(keyword));
        }
        [HttpGet("locphim")]
        public IActionResult Locphhim(int movietypeid)
        {
            return Ok(_movieService.LocPhimTheoTheLoai(movietypeid));
        }

    }
}
