using DATN.Entities;
using DATN.Payload.DTO;

namespace DATN.Payload.Converter
{
    public class SeatStatusConverter
    {
        public SeatStatusDTO EntityToDTO(SeatStatus seatStatus)
        {
            return new SeatStatusDTO
            {
                Id = seatStatus.Id,
                Code = seatStatus.Code,
                NameStatus = seatStatus.NameStatus,
            };
        }
    }
}
