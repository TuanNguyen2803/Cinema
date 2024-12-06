using DATN.DataContext;
using DATN.Entities;
using DATN.Payload.Converter;
using DATN.Payload.DTO;
using DATN.Payload.Request.CinemaRequest;
using DATN.Payload.Response;
using DATN.Service.Interface;
using System.Diagnostics.Eventing.Reader;

namespace DATN.Service.Implement
{
    public class CinemaService : ICinemaService
    {
        private readonly AppDbContext dbContext;
        private readonly CinemaConverter cinemaConverter;
        private readonly ResponseObject<CinemaDTO> responseObject;
        private readonly IConfiguration _configuration;

        public CinemaService(AppDbContext dbContext, CinemaConverter cinemaConverter, ResponseObject<CinemaDTO> responseObject, IConfiguration configuration)
        {
            this.dbContext = dbContext;
            this.cinemaConverter = cinemaConverter;
            this.responseObject = responseObject;
            _configuration = configuration;
        }

        public IQueryable<CinemaDTO> Getall()
        {
            IQueryable<Cinema> check=dbContext.cinemas.AsQueryable();
            var result = check.Select(x => cinemaConverter.EntityToDTO(x));
            return result;
        }

        public IQueryable<CinemaDTO> laytheoid(int cinemaid)
        {
            IQueryable<Cinema> check=dbContext.cinemas.Where(x=>x.Id==cinemaid).AsQueryable();
            var result=check.Select(x=>cinemaConverter.EntityToDTO(x));
            return result;
        }

        public ResponseObject<CinemaDTO> Sua(SuaCinemaRequest request)
        {
            Cinema cinema = dbContext.cinemas.FirstOrDefault(x => x.Id == request.ID);
            if (cinema is null) 
            {
                return responseObject.ResponseError(StatusCodes.Status404NotFound, $"Không tồn tại cinemaid {request.ID} ", null);
            }
            else
            {
                cinema.Address = request.Address;
                cinema.Description = request.Description;
                cinema.Code= "Cinema__" + DateTime.Now.Ticks.ToString() + "_xyz_" + new Random().Next(100, 999);
                cinema.NameOfCinema= request.NameOfCinema;
                cinema.IsActive = request.IsActive;
                dbContext.cinemas.Update(cinema);
                dbContext.SaveChanges();
                return responseObject.ResponseSuccess("Sửa thành công",cinemaConverter.EntityToDTO(cinema));
               
            }
        }
        public ResponseObject<CinemaDTO> Them(ThemCinemaRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Address)
                || string.IsNullOrWhiteSpace(request.Description)
                || string.IsNullOrWhiteSpace(request.NameOfCinema)
                )
            {
                return responseObject.ResponseError(StatusCodes.Status404NotFound, "Vui lòng điền đầy đủ thông tin", null);
            }
            else
            {
                Cinema cinema = new Cinema();
                cinema.Address = request.Address;
                cinema.Description = request.Description;
                cinema.Code = "Cinema__" + DateTime.Now.Ticks.ToString() + "_xyz_" + new Random().Next(100, 999);
                cinema.NameOfCinema = request.NameOfCinema;
                cinema.IsActive=true;

                dbContext.cinemas.Add(cinema);
                dbContext.SaveChanges();
                return responseObject.ResponseSuccess("Thêm thành công", cinemaConverter.EntityToDTO(cinema));
            }
        }
        public ResponseObject<CinemaDTO> Xoa(int cinemaid)
        {
            Cinema cinema = dbContext.cinemas.SingleOrDefault(x => x.Id == cinemaid);
            if (cinema is null)
            {
                return responseObject.ResponseError(StatusCodes.Status404NotFound, $"Không tồn tại cinema {cinemaid}", null);
            }
            else
            {
                cinema.IsActive = false;
                dbContext.cinemas.Update(cinema);
                dbContext.SaveChanges();
                return responseObject.ResponseSuccess("Xóa thành công", null);
            }
        }
        public IQueryable<CinemaDTO> TimKiem(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return new List<CinemaDTO>().AsQueryable(); // Trả về một danh sách trống nếu không có keyword
            }

            // Chuẩn hóa từ khóa tìm kiếm để loại bỏ các khoảng trắng không cần thiết
            keyword = keyword.Trim().ToLower();

            // Tìm kiếm theo tên rạp, địa chỉ hoặc mô tả
            IQueryable<Cinema> check = dbContext.cinemas
                .Where(x => x.NameOfCinema.ToLower().Contains(keyword)
                            || x.Address.ToLower().Contains(keyword)
                            || x.Description.ToLower().Contains(keyword));

            var result = check.Select(x => cinemaConverter.EntityToDTO(x));

            return result;
        }


    }
}
