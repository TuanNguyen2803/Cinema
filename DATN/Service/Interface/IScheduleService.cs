using DATN.Payload.DTO;
using DATN.Payload.Request.ScheduleRequest;
using DATN.Payload.Request.SeatRequest;
using DATN.Payload.Response;

namespace DATN.Service.Interface
{
    public interface IScheduleService
    {
        ResponseObject<ScheduleDTO> Them(ThemScheduleRequest request);
        ResponseObject<List<ScheduleDTO>> ThemLichChieuTuFileExcel(IFormFile file);
        ResponseObject<ScheduleDTO> Sua(SuaScheduleRequest request);
        ResponseObject<ScheduleDTO> Xoa(int scheduleid);
        IQueryable<ScheduleDTO> Getall(int movieid, int? cinemaid);
        IQueryable<ScheduleDTO> GetbyRap(int movieid, int? cinemaid);
        IQueryable<ScheduleDTO> hienthitheoid(int scheduleid);
        IQueryable<ScheduleDTO> TimKiemLichChieu(string keyword);
        IQueryable<ScheduleDTO> GetSchedules(int? cinemaId, int? roomId, int? movieId);
    }
}
