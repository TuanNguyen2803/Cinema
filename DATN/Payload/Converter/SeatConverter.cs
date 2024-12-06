using DATN.Entities;
using DATN.Payload.DTO;

namespace DATN.Payload.Converter
{
    public class SeatConverter
    {
        public SeatDTO EntityToDTO( Seat seat )
        {
            return new SeatDTO
            {
                Id = seat.Id,
                Number = seat.Number,
                SeatStatusId = seat.SeatStatusId,
                Line = seat.Line,
                RoomId = seat.RoomId,
                IsActive = seat.IsActive,
                SeatTypeId = seat.SeatTypeId,
            };
        }
    }
}
