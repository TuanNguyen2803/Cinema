using DATN.Handle.InputMovie;
using DATN.Payload.DTO;
using DATN.Payload.Request.MovieRequest;
using DATN.Payload.Response;

namespace DATN.Service.Interface
{
    public interface IMovieService
    {
        Task<ResponseObject<MovieDTO>> Them(ThemMovieRequest request);
        Task<ResponseObject<List<MovieDTO>>> ImportMoviesFromExcel(IFormFile excelFile);
        Task<ResponseObject<MovieDTO>> Sua(SuaMovieRequest request);
        ResponseObject<MovieDTO> Xoa(int movieid);
        IQueryable<MovieDTO> HienThiPhim(InputMovie input, int pageSize, int pageNumber);
        IQueryable<MovieDTO> HienThiTatCaPhim();
        IQueryable<MovieDTO> Hienthithongtinphim(int movieid);
        IQueryable<MovieDTO> TimKiemPhim(string keyword);
        IQueryable<MovieDTO> LocPhimTheoTheLoai(int movieTypeId);
    }
}
