using Azure;
using DATN.Payload.DTO;
using DATN.Payload.Request.CinemaRequest;
using DATN.Payload.Response;

namespace DATN.Service.Interface
{
    public interface ICinemaService
    {
        ResponseObject<CinemaDTO> Them(ThemCinemaRequest request);
        ResponseObject<CinemaDTO> Sua(SuaCinemaRequest request);
        ResponseObject<CinemaDTO> Xoa(int cinemaid);
        IQueryable<CinemaDTO> Getall();
        IQueryable<CinemaDTO> laytheoid(int cinemaid);
        IQueryable<CinemaDTO> TimKiem(string keyword);
    }
}
