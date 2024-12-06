using DATN.Entities;

namespace DATN.Payload.DTO
{
    public class TicketDTO
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public int ScheduleId { get; set; }
        public int SeatId { get; set; }
        public double PriceTicket { get; set; }
        public bool? IsActive { get; set; } 
    }
    public class TicketSalesDTO
    {
        public int SeatId { get; set; }
        public int TotalTicketsSold { get; set; }
    }
}
