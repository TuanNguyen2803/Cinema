using DATN.Payload.DTO;
using DATN.Payload.Request.Room;
using DATN.Payload.Request.SeatRequest;
using DATN.Payload.Response;

namespace DATN.Service.Interface
{
    public interface ISeatService
    {
        ResponseObject<SeatDTO> Them(ThemSeatRequest request);
        ResponseObject<List<SeatDTO>> ThemGheTuFileExcel(IFormFile file);
        ResponseObject<SeatDTO> Sua(SuaSeatRequest request);
        ResponseObject<SeatDTO> Xoa(int seatid);
        IQueryable<SeatDTO> Hienthi();
        IQueryable<SeatDTO> hienthitheophong(int roomid);
        IQueryable<SeatDTO> hienthitheoid(int seatid);
    }
}
