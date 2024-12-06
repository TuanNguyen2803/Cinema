using DATN.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DATN.Controller
{
    [ApiController]
    public class MovieTypeController : ControllerBase
    {
        private readonly IMovieTypeService _movieTypeService;
        public MovieTypeController(IMovieTypeService movieTypeService)
        {
            _movieTypeService = movieTypeService;
        }
        [HttpGet("Hienthimovietype")]
        public IActionResult hienthimovietypt()
        {
            return Ok(_movieTypeService.GetAll());
        }
    }
}
