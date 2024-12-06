using DATN.Payload.DTO;
using DATN.Payload.Request.CinemaRequest;
using DATN.Payload.Request.Room;
using DATN.Payload.Response;

namespace DATN.Service.Interface
{
    public interface IRoomService
    {
        ResponseObject<RoomDTO> Them(ThemRoomRequest request);
        ResponseObject<List<RoomDTO>> ImportRoomFromExcel(IFormFile excelFile);
        ResponseObject<RoomDTO> Sua(SuaRoomRequest request);
        ResponseObject<RoomDTO> Xoa(int roomid);
        IQueryable<RoomDTO> GetRooms(int? cinemaid);
        IQueryable<RoomDTO> Hienthitheoid(int roomid);
        IQueryable<RoomDTO> Hienthiroomtat();
        IQueryable<RoomDTO> TimKiemRoom(string keyword);
    }
}
