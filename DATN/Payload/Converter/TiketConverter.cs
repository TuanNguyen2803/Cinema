using DATN.Entities;
using DATN.Payload.DTO;

namespace DATN.Payload.Converter
{
    public class TiketConverter
    {
        public TicketDTO EntityToDTO(Ticket ticket)
        {
            return new TicketDTO
            {
                Id = ticket.Id,
                Code = ticket.Code,
                ScheduleId = ticket.ScheduleId,
                SeatId = ticket.SeatId,
                PriceTicket = ticket.PriceTicket,
                IsActive = ticket.IsActive,
            };
        }
    }
}
