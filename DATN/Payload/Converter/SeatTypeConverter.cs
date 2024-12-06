using DATN.Entities;
using DATN.Payload.DTO;

namespace DATN.Payload.Converter
{
    public class SeatTypeConverter
    {
        public SeatTypeDTO EntityToDTO(SeatType seatType)
        {
            return new SeatTypeDTO
            {
                Id = seatType.Id,
                NameType=seatType.NameType,
            };
        }
    }
}
