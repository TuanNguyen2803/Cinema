using System.Net.Sockets;

namespace DATN.Entities
{
    public class Seat : BaseEntity
    {
        public int Number { get; set; }
        public int SeatStatusId { get; set; }
        public string Line { get; set; }
        public int RoomId { get; set; }
        public bool? IsActive { get; set; } = true;
        public int SeatTypeId {  get; set; }
        public SeatStatus? SeatStatus { get; set; }
        public Room? Room { get; set; }
        public SeatType? SeatType { get; set; }
        public IEnumerable<Ticket> Tickets { get; set; }

    }
}
