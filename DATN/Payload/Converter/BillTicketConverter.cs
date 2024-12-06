using DATN.Entities;
using DATN.Payload.DTO;

namespace DATN.Payload.Converter
{
    public class BillTicketConverter
    {
        public BillTicketDTO EntityToDTO(BillTicket billTicket)
        {
            return new BillTicketDTO
            {
                Id = billTicket.Id,
                BillId = billTicket.BillId,
                Quantity = billTicket.Quantity,

            };
        }
    }
}
