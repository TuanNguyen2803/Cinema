using DATN.Entities;
using DATN.Payload.DTO;
using DATN.Payload.Request.BillFoodRequest;
using NetMQ.Sockets;

namespace DATN.Payload.Request.BillRequest
{
    public class CreateBillRequest
    {
        public int MovieId { get; set; }
        public int CinemaId { get; set; }
        public int RoomId { get; set; }
        public int ScheduleId { get; set; }
    
        public List<SeatRequest> SeatList { get; set; }
        public  List<CreateBillFoodRequest> billFoods { get; set; }
        
    }
}
