using DATN.DataContext;
using DATN.Entities;
using DATN.Payload.Converter;
using DATN.Payload.DTO;
using DATN.Payload.Request.MovieTypeRequest;
using DATN.Payload.Response;
using DATN.Service.Interface;

namespace DATN.Service.Implement
{
    public class MovieTypeService : IMovieTypeService
    {
        private readonly AppDbContext _context;
        private readonly ResponseObject<MovieTypeDTO> responseObject;
        private readonly MovieTypeConverter movieTypeConverter;
        private readonly IConfiguration _configuration;

        public MovieTypeService(AppDbContext context, ResponseObject<MovieTypeDTO> responseObject, MovieTypeConverter movieTypeConverter, IConfiguration configuration)
        {
            _context = context;
            this.responseObject = responseObject;
            this.movieTypeConverter = movieTypeConverter;
            _configuration = configuration;
        }

        public ResponseObject<MovieTypeDTO> Create(ThemMovieTypeRequest request)
        {
            if(string.IsNullOrWhiteSpace(request.MovieTypeName))
              
            {
                return responseObject.ResponseError(StatusCodes.Status404NotFound, "Vui lòng điền đầy đủ thông tin", null);
            }
            MovieType movieType = new MovieType();
            movieType.MovieTypeName = request.MovieTypeName;
            movieType.IsActive = true;
            _context.movieTypes.Add(movieType);
            _context.SaveChanges();
            return responseObject.ResponseSuccess("Thêm thành công",movieTypeConverter.EntityToDTO(movieType));

        }

        public ResponseObject<MovieTypeDTO> Delete(int movieid)
        {
            var check = _context.movieTypes.Find(movieid);
            if(check != null)
            {
                _context.movieTypes.Remove(check);
                _context.SaveChanges();
                return responseObject.ResponseSuccess("Xóa thành công", null);
            }
            return responseObject.ResponseError(StatusCodes.Status404NotFound, $"Không tồn tại movietype có id {movieid}", null);
        }

        public IQueryable<MovieTypeDTO> GetAll()
        {
            var check= _context.movieTypes.AsQueryable();
            var result=check.Select(x=>movieTypeConverter.EntityToDTO(x));
            return result;
        }

        public ResponseObject<MovieTypeDTO> Update(SuaMovieTypeRequest request)
        {
            var check = _context.movieTypes.Find(request.Id);
            if (check != null)
            {
                check.MovieTypeName = request.MovieTypeName;
                check.IsActive=request.IsActive;
                _context.movieTypes.Update(check);
                _context.SaveChanges();
                return responseObject.ResponseSuccess("Sửa thành công", null);
            }
            return responseObject.ResponseError(StatusCodes.Status404NotFound, $"Không tồn tại movietype có id {request.Id}", null);
        }
    }
}
